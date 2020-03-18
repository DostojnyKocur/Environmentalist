using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.EnvWriter;
using Environmentalist.Services.LogicProcessor;
using Environmentalist.Services.TemplateReader;
using Environmentalist.Validators.FileValidator;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Environmentalist
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            CreateLogger();

            Log.Logger.Information("Starting application");

            RegisterServices();

            try
            {
                var logicProcessor = _serviceProvider.GetService<ILogicProcessor>();
                var templateReader = _serviceProvider.GetService<ITemplateReader>();
                var envWriter = _serviceProvider.GetService<IEnvWriter>();

                var template = await templateReader.Read(args[0]);
                var config = await templateReader.Read(args[1]);

                var output = logicProcessor.Process(template, config);

                await envWriter.Write(output, args[2]);
            }
            catch(Exception exception)
            {
                Log.Logger.Error(exception, "An error occured");
            }

            Log.Logger.Information("Terminating application");

            DisposeServices();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();

            builder.RegisterType<FileSystem>().As<IFileSystem>();

            builder.RegisterType<FileValidator>().As<IFileValidator>();

            builder.RegisterType<DiskService>().As<IDiskService>();
            builder.RegisterType<TemplateReader>().As<ITemplateReader>();
            builder.RegisterType<EnvWriter>().As<IEnvWriter>();
            builder.RegisterType<LogicProcessor>().As<ILogicProcessor>();

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
