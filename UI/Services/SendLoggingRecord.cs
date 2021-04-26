using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stl.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.UI.Services
{
    //[Service(typeof(ISendLoggingRecord), Lifetime = ServiceLifetime.Scoped, Scope = Program.ClientSideScope)]
    public class SendLoggingRecord : ISendLoggingRecord
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<SendLoggingRecord> log;

        public SendLoggingRecord(IServiceProvider provider, ILogger<SendLoggingRecord> log)
        {
            this.provider = provider;
            this.log = log;
            log.LogInformation("Service initialized");
        }

        public async Task EnQuee(LoggingRecord loggingRecord)
        {
            log.LogWarning(loggingRecord.ToString());
        }

    }

    public interface ISendLoggingRecord
    {
        Task EnQuee(LoggingRecord loggingRecord);
    }
}
