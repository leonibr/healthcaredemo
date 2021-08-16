using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stl.DependencyInjection;
//using Stl.RegisterAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.UI.Services
{
  //  [RegisterService(typeof(ISendLoggingRecord), Lifetime = ServiceLifetime.Scoped, Scope = Program.ClientSideScope)]
    public class SendLoggingRecord : ISendLoggingRecord
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<SendLoggingRecord> log;

        private Subject<LoggingRecord> loggingRecord = new Subject<LoggingRecord>();

        public IObservable<LoggingRecord> LoggingRecord => loggingRecord.AsObservable();

        public void EnQuee(LoggingRecord record) => loggingRecord.OnNext(record);

        public SendLoggingRecord(IServiceProvider provider, ILogger<SendLoggingRecord> log)
        {
            this.provider = provider;
            this.log = log;
            log.LogInformation("Service initialized");
        }


    }

    public interface ISendLoggingRecord
    {
        IObservable<LoggingRecord> LoggingRecord { get; }

        void EnQuee(LoggingRecord record);
    }
}
