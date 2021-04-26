using FusionDemo.HealthCentral.Domain;
using Stl.Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Abstractions
{
    public interface IRequestLoggingService
    {
        Task AddLoggingRecord(LoggingRecord loggingRecord);
        [ComputeMethod]
        Task<IEnumerable<LoggingRecord>> GetLatest(CancellationToken cancellationtoken);
    }
}
