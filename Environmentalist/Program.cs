using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.EnvWriter;
using Environmentalist.Services.LogicProcessor;
using Environmentalist.Services.Pbkdf2Service;
using Environmentalist.Services.Readers.ConfigurationReader;
using Environmentalist.Services.Readers.EnvironmentVariableReader;
using Environmentalist.Services.Readers.KeePassReader;
using Environmentalist.Services.Readers.ProfileReader;
using Environmentalist.Services.Readers.TemplateReader;
using Environmentalist.Services.Repositories.ConfigurationRepository;
using Environmentalist.Services.Repositories.Pbkdf2Repository;
using Environmentalist.Services.Repositories.ProfileRepository;
using Environmentalist.Services.Repositories.TemplateRepository;
using Environmentalist.Validators;
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
        private const string Usage = @"Usage: Environmentalist <path to config file .conf> or Environmentalist --encrypt <input file> <output file>";
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
                    if (args.Length == 1)
                    {
                        var configFilePath = args[0];

                        await PrepareConfiguration(configFilePath);
                    }
                    else if (args.Length == 3)
                    {
                        var sourcePath = args[1];
                        var outputPath = args[2];

                        await EncryptFile(sourcePath, outputPath);
                    }
                }
                catch (Exception exception)
                {
                    Log.Logger.Error(exception, "During processing an error occured");
                }
            }

            Log.Logger.Debug("Terminating application");

            DisposeServices();
        }

        private static async Task PrepareConfiguration(string configFilePath)
        {
            var configurationRepository = _serviceProvider.GetService<IConfigurationRepository>();
            var configuration = await configurationRepository.GetConfiguration(configFilePath);

            var templateRepository = _serviceProvider.GetService<ITemplateRepository>();
            var template = await templateRepository.GetTemplate(configuration.TemplatePath);

            var profileRepository = _serviceProvider.GetService<IProfileRepository>();
            var profile = await profileRepository.GetProfile(configuration.ProfilePath);

            var pbkdf2Repository = _serviceProvider.GetService<IPbkdf2Repository>();
            var protectedFile = await pbkdf2Repository.Decrypt(configuration.ProtectedFilePath, configuration.ProtectedFileEntropy);

            var reader = _serviceProvider.GetService<IKeePassReader>();
            var kdbx = reader.ReadDatabase(configuration.SecureVaultPath, configuration.SecureVaultPass);

            var logicProcessor = _serviceProvider.GetService<ILogicProcessor>();
            var output = logicProcessor.Process(template, profile, protectedFile, kdbx);

            var envWriter = _serviceProvider.GetService<IEnvWriter>();
            await envWriter.Write(output, configuration.ResultPath, configuration.TemplatePath);
        }

        private static async Task EncryptFile(string sourcePath, string outputPath)
        {
            var pbkdf2Repository = _serviceProvider.GetService<IPbkdf2Repository>();
            await pbkdf2Repository.Encrypt(sourcePath, outputPath);
        }

        #region *** Register ***

        private static void RegisterTypes()
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();

            RegisterInfrastructure(builder);
            RegisterWriters(builder);
            RegisterLogics(builder);
            RegisterRepositories(builder);
            RegisterValidators(builder);
            RegisterReaders(builder);

            builder.Populate(collection);
            var appContainer = builder.Build();
            _serviceProvider = new AutofacServiceProvider(appContainer);
        }

        private static void RegisterInfrastructure(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<DiskService>().As<IDiskService>().SingleInstance();
            builder.RegisterType<Pbkdf2Service>().As<IPbkdf2Service>().SingleInstance();
        }

        private static void RegisterWriters(ContainerBuilder builder)
        {
            builder.RegisterType<EnvWriter>().As<IEnvWriter>().SingleInstance();
        }

        private static void RegisterLogics(ContainerBuilder builder)
        {
            builder.RegisterType<LogicProcessor>().As<ILogicProcessor>().SingleInstance();
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationRepository>().As<IConfigurationRepository>().SingleInstance();
            builder.RegisterType<TemplateRepository>().As<ITemplateRepository>().SingleInstance();
            builder.RegisterType<ProfileRepository>().As<IProfileRepository>().SingleInstance();
            builder.RegisterType<Pbkdf2Repository>().As<IPbkdf2Repository>().SingleInstance();
        }

        private static void RegisterValidators(ContainerBuilder builder)
        {
            builder.RegisterType<FileValidator>().As<IFileValidator>().SingleInstance();
            builder.RegisterType<ObjectValidator>().As<IObjectValidator>().SingleInstance();
            builder.RegisterType<StringValidator>().As<IStringValidator>().SingleInstance();
            builder.RegisterType<Validators.Validators>().As<IValidators>().SingleInstance();
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
