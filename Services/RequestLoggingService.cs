using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Microsoft.Extensions.Logging;
using Stl.Async;
using Stl.Fusion;
using Stl.RegisterAttributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Services
{
    [RegisterService(typeof(IRequestLoggingService))]
    public class RequestLoggingService: IRequestLoggingService
    {
        readonly ILogger<RequestLoggingService> logger;
        private IList<LoggingRecord> database = new List<LoggingRecord>();

        public RequestLoggingService(ILogger<RequestLoggingService> logger)
        {
            this.logger = logger;
        }
        public Task AddLoggingRecord(LoggingRecord loggingRecord)
        {
            database.Add(loggingRecord);
            using (Computed.Invalidate())
                GetLatest().Ignore();
            return Task.CompletedTask;
        }

        [ComputeMethod]
        public virtual async Task<IEnumerable<LoggingRecord>> GetLatest(CancellationToken cancellationtoken = default)
        {
            
            var records = database.Take(25).ToArray();
            return await Task.FromResult(records);
        }
    }
}