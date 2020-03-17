using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.EnvWriter;
using Environmentalist.Services.TemplateReader;
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

            var templateReader = _serviceProvider.GetService<ITemplateReader>();
            var envWriter = _serviceProvider.GetService<IEnvWriter>();

            var template = await templateReader.Read(args[0]);
            await envWriter.Write(template, args[1]);

            Log.Logger.Information("Terminating application");

            DisposeServices();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            var builder = new ContainerBuilder();

            builder.RegisterType<DiskService>().As<IDiskService>();
            builder.RegisterType<TemplateReader>().As<ITemplateReader>();
            builder.RegisterType<EnvWriter>().As<IEnvWriter>();

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
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
