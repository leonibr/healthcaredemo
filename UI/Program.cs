using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using Pluralize.NET;
using Stl.Fusion;
using Stl.Fusion.Client;
using Stl.OS;
using Stl.DependencyInjection;
using Stl.Fusion.Blazor;
using FusionDemo.HealthCentral.Abstractions;
using Stl.Fusion.Bridge;
using FusionDemo.HealthCentral.UI.Services;
using Stl.Fusion.UI;
using Stl.Fusion.Extensions;

namespace FusionDemo.HealthCentral.UI
{
    public class Program
    {
        public const string ClientSideScope = nameof(ClientSideScope);

        public static Task Main(string[] args)
        {
            if (OSInfo.Kind != OSKind.WebAssembly)
                throw new ApplicationException("This app runs only in browser.");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder.Services, builder);
            // builder.RootComponents.Add<App>("#app");
            var host = builder.Build();
            host.Services.HostedServices().Start();
            return host.RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services, WebAssemblyHostBuilder builder)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Trace);

            var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiBaseUri = new Uri($"{baseUri}api/");

            // This method registers services marked with any of ServiceAttributeBase descendants, including:
            // [Service], RegisterService, [RestEaseReplicaService], [LiveStateUpdater]
            //services.UseRegisterAttributeScanner(ClientSideScope)
            //    .RegisterFrom(Assembly.GetExecutingAssembly());
         

            var fusion = services.AddFusion();
            var fusionClient = fusion.AddRestEaseClient();
            fusionClient.ConfigureWebSocketChannel(c => new()
            {
                BaseUri = baseUri,
                LogLevel = LogLevel.Information,
                MessageLogLevel = LogLevel.Trace,
            });
            fusionClient.ConfigureHttpClient((c, name, o) => {
                var isFusionClient = (name ?? "").StartsWith("Stl.Fusion");
                var clientBaseUri = isFusionClient ? baseUri : apiBaseUri;
                o.HttpClientActions.Add(client => client.BaseAddress = clientBaseUri);
            });

            fusion.AddBlazorUIServices();

            fusionClient.AddReplicaService<ITimeService, ITimeClient>();
            fusionClient.AddReplicaService<IPatientService, IPatientClient>();
            fusionClient.AddReplicaService<INotificationService, INotificationClient>();
            fusionClient.AddReplicaService<IRequestLoggingService, IRequestLoggingClient>();
            fusionClient.AddReplicaService<IPainelComposerService, IPainelComposerClient>();


            ConfigureSharedServices(services);
        }

        public static void ConfigureSharedServices(IServiceCollection services)
        {

            services.AddMudServices();
            var fusion = services.AddFusion();
            fusion.AddFusionTime();
            fusion.AddBackendStatus();
            // Default delay for update delayers
            services.AddTransient<IUpdateDelayer>(c => new UpdateDelayer(c.UIActionTracker(), 0.1 ));

            services.AddSingleton<IPluralize, Pluralizer>();

            // This method registers services marked with any of ServiceAttributeBase descendants, including:
            // [Service], RegisterService, [RestEaseReplicaService], [LiveStateUpdater]
            //services.UseRegisterAttributeScanner().RegisterFrom(Assembly.GetExecutingAssembly());
        }
    }
}
