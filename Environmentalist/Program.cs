using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Environmentalist.Services.ConfigurationReader;
using Environmentalist.Services.ConfigurationRepository;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Services.EnvWriter;
using Environmentalist.Services.KeePassReader;
using Environmentalist.Services.LogicProcessor;
using Environmentalist.Services.ProfileReader;
using Environmentalist.Services.TemplateReader;
using Environmentalist.Services.TemplateRepository;
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

            RegisterTypes();

            if (args.Length == 0)
            {
                Log.Logger.Information(Usage);
            }
            else
            {
                try
                {
                    var configFilePath = args[0];

                    var configurationRepository = _serviceProvider.GetService<IConfigurationRepository>();
                    var configuration = await configurationRepository.GetConfiguration(configFilePath);

                    var templateRepository = _serviceProvider.GetService<ITemplateRepository>();
                    var template = await templateRepository.GetTemplate(configuration.TemplatePath);

                    var envVariablesReader = _serviceProvider.GetService<IEnvironmentVariableReader>();

                    var profileReader = _serviceProvider.GetService<IProfileReader>();
                    var profile = await profileReader.Read(configuration.ProfilePath);

                    var configEnvVariables = profileReader.ExtractEnvironmentVariables(profile);

                    var envVariablesValues = envVariablesReader.Read(configEnvVariables);

                    var reader = _serviceProvider.GetService<IKeePassReader>();
                    var kdbx = reader.ReadDatabase(configuration.SecureVaultPath, configuration.SecureVaultPass);

                    var logicProcessor = _serviceProvider.GetService<ILogicProcessor>();
                    var output = logicProcessor.Process(template, profile, envVariablesValues, kdbx);

                    var envWriter = _serviceProvider.GetService<IEnvWriter>();
                    await envWriter.Write(output, configuration.ResultPath, configuration.TemplatePath);
                }
                catch (Exception exception)
                {
                    Log.Logger.Error(exception, "During processing an error occured");
                }
            }

            Log.Logger.Debug("Terminating application");

            DisposeServices();
        }

        #region *** Register ***

        private static void RegisterTypes()
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();

            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();       
            builder.RegisterType<DiskService>().As<IDiskService>().SingleInstance();          
            builder.RegisterType<EnvWriter>().As<IEnvWriter>().SingleInstance();
            builder.RegisterType<LogicProcessor>().As<ILogicProcessor>().SingleInstance();
            builder.RegisterType<ConfigurationRepository>().As<IConfigurationRepository>().SingleInstance();
            builder.RegisterType<TemplateRepository>().As<ITemplateRepository>().SingleInstance();

            RegisterValidators(builder);
            RegisterReaders(builder);

            builder.Populate(collection);
            var appContainer = builder.Build();
            _serviceProvider = new AutofacServiceProvider(appContainer);
        }

        private static void RegisterValidators(ContainerBuilder builder)
        {
            builder.RegisterType<FileValidator>().As<IFileValidator>().SingleInstance();
            builder.RegisterType<ObjectValidator>().As<IObjectValidator>().SingleInstance();
            builder.RegisterType<StringValidator>().As<IStringValidator>().SingleInstance();
        }

        private static void RegisterReaders(ContainerBuilder builder)
        {
            builder.RegisterType<TemplateReader>().As<ITemplateReader>().SingleInstance();
            builder.RegisterType<ProfileReader>().As<IProfileReader>().SingleInstance();
            builder.RegisterType<KeePassReader>().As<IKeePassReader>().SingleInstance();
            builder.RegisterType<ConfigurationReader>().As<IConfigurationReader>().SingleInstance();
            builder.RegisterType<EnvironmentVariableReader>().As<IEnvironmentVariableReader>().SingleInstance();
        }

        #endregion *** Register ***

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
