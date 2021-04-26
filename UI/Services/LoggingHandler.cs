using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stl.DependencyInjection;
using Stl.Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.UI.Services
{
   [Service(Lifetime = ServiceLifetime.Transient, Scope = Program.ClientSideScope)]
    public class LoggingHandler : DelegatingHandler
    {

        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly IServiceProvider serviceProvider;
        private readonly ISendLoggingRecord sender;
        private readonly ILogger<LoggingHandler> logger;

        public LoggingHandler(IServiceProvider serviceProvider, ISendLoggingRecord sender, ILogger<LoggingHandler> logger)
        {
            this.serviceProvider = serviceProvider;
            this.sender = sender;
            this.logger = logger;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           // var sender = serviceProvider.GetRequiredService<ISendLoggingRecord>();
            stopwatch.Restart();
            var response = await base.SendAsync(request, cancellationToken);
           stopwatch.Stop();
            var loggingRecord = new LoggingRecord()
            {
                Duration = stopwatch.ElapsedMilliseconds,
                Method = request.Method.Method.ToUpperInvariant(),
                Path = request.RequestUri?.PathAndQuery?.ToString(),
                Browser = string.Empty,
                When = DateTime.UtcNow,
                Status = (int)response.StatusCode
            };

           logger.LogWarning($"Request Logging {stopwatch.ElapsedMilliseconds}ms");
           await sender.EnQuee(loggingRecord);
            return response;
        }
    }
}
