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
            builder.RootComponents.Add<App>("#app");
            var host = builder.Build();

            host.Services.HostedServices().Start();
            return host.RunAsync();
        }

        public static void ConfigureServices(IServiceCollection services, WebAssemblyHostBuilder builder)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Warning);

            var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiBaseUri = new Uri($"{baseUri}api/");
           
            // This method registers services marked with any of ServiceAttributeBase descendants, including:
            // [Service], [ComputeService], [RestEaseReplicaService], [LiveStateUpdater]
            services.UseAttributeScanner(ClientSideScope)
                .AddServicesFrom(Assembly.GetExecutingAssembly());

            services.AddScoped<ISendLoggingRecord, SendLoggingRecord>();
            var fusion = services.AddFusion();
            var fusionClient = fusion.AddRestEaseClient(
                (c, o) => {
                    o.BaseUri = baseUri;
                    o.MessageLogLevel = LogLevel.Information;
                    
                })
                .ConfigureHttpClientFactory(
                (c, name, o) => {
                   
                    var isFusionClient = (name ?? "").StartsWith("Stl.Fusion");
                    var clientBaseUri = isFusionClient ? baseUri : apiBaseUri;
                    o.HttpClientActions.Add(client => client.BaseAddress = clientBaseUri);                  
                    o.HttpMessageHandlerBuilderActions.Add(handler =>
                    {
                        //using var scope = handler.Services.CreateScope();
                       
                       var loggingHandler =  handler.Services.GetRequiredService<LoggingHandler>();
                      
                        Console.WriteLine("Got handler!!");
                        handler.AdditionalHandlers.Add(loggingHandler);
                       
                      
                    });
                })
                
                ;
           // var fusionAuth = fusion.AddAuthentication().AddRestEaseClient().AddBlazor();


            ConfigureSharedServices(services);
        }

        public static void ConfigureSharedServices(IServiceCollection services)
        {

            services.AddMudServices();
            // Default delay for update delayers
            services.AddSingleton<IUpdateDelayer>(c => new UpdateDelayer(0.1));

            services.AddSingleton<IPluralize, Pluralizer>();

            // This method registers services marked with any of ServiceAttributeBase descendants, including:
            // [Service], [ComputeService], [RestEaseReplicaService], [LiveStateUpdater]
            services.UseAttributeScanner().AddServicesFrom(Assembly.GetExecutingAssembly());
        }
    }
}
