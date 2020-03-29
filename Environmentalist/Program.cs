﻿using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Environmentalist.Services.ConfigurationReader;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Services.EnvWriter;
using Environmentalist.Services.KeePassReader;
using Environmentalist.Services.LogicProcessor;
using Environmentalist.Services.TemplateReader;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Environmentalist
{
    class Program
    {
        private const string Usage = @"Usage: Environmentalist <path to config file .conf>";
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            CreateLogger();

            Log.Logger.Debug("Starting application");

            RegisterServices();

            if (args.Length == 0)
            {
                Log.Logger.Information(Usage);
            }
            else
            {
                try
                {
                    var configFilePath = args[0];
                    var configReader = _serviceProvider.GetService<IConfigurationReader>();
                    var configuration = await configReader.Read(configFilePath);
                    var configurationEnvVariables = configReader.ExtractEnvironmentVariables(configuration);

                    var envVariablesReader = _serviceProvider.GetService<IEnvironmentVariableReader>();
                    var configEnvVariablesValues = envVariablesReader.Read(configurationEnvVariables);
                    configuration = configReader.ProcessEnvironmentVariables(configuration, configEnvVariablesValues);

                    var templateReader = _serviceProvider.GetService<ITemplateReader>();
                    var template = await templateReader.Read(configuration.TemplatePath);
                    var config = await templateReader.Read(configuration.ConfigPath);
                    var templateEnvVariables = templateReader.ExtractEnvironmentVariables(template);
                    var configEnvVariables = templateReader.ExtractEnvironmentVariables(config);
                    var envVariables = templateEnvVariables.Concat(configEnvVariables).ToList();
                    var envVariablesValues = envVariablesReader.Read(envVariables);

                    var reader = _serviceProvider.GetService<IKeePassReader>();
                    var kdbx = reader.ReadDatabase(configuration.SecureVaultPath, configuration.SecureVaultPass);

                    var logicProcessor = _serviceProvider.GetService<ILogicProcessor>();
                    var output = logicProcessor.Process(template, config, envVariablesValues, kdbx);

                    var envWriter = _serviceProvider.GetService<IEnvWriter>();
                    await envWriter.Write(output, configuration.ResultPath);
                }
                catch (Exception exception)
                {
                    Log.Logger.Error(exception, "During processing an error occured");
                }
            }

            Log.Logger.Debug("Terminating application");

            DisposeServices();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();

            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();

            builder.RegisterType<FileValidator>().As<IFileValidator>().SingleInstance();
            builder.RegisterType<ObjectValidator>().As<IObjectValidator>().SingleInstance();
            builder.RegisterType<StringValidator>().As<IStringValidator>().SingleInstance();

            builder.RegisterType<DiskService>().As<IDiskService>().SingleInstance();

            builder.RegisterType<TemplateReader>().As<ITemplateReader>().SingleInstance();
            builder.RegisterType<KeePassReader>().As<IKeePassReader>().SingleInstance();
            builder.RegisterType<ConfigurationReader>().As<IConfigurationReader>().SingleInstance();
            builder.RegisterType<EnvironmentVariableReader>().As<IEnvironmentVariableReader>().SingleInstance();

            builder.RegisterType<EnvWriter>().As<IEnvWriter>().SingleInstance();
            builder.RegisterType<LogicProcessor>().As<ILogicProcessor>().SingleInstance();

            builder.Populate(collection);
            var appContainer = builder.Build();
            _serviceProvider = new AutofacServiceProvider(appContainer);
        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        }

        private static void DisposeServices()
        {
            if (!(_serviceProvider is null) && (_serviceProvider is IDisposable))
            { 
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
