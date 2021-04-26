using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Stl.Async;
using Stl.Fusion;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Services
{
    [ComputeService(typeof(IRequestLoggingService))]
    public class RequestLoggingService: IRequestLoggingService
    {
        private IList<LoggingRecord> database = new List<LoggingRecord>();


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