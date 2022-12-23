using System;
using System.Threading;
using System.Threading.Tasks;
using Stl.Fusion;
using FusionDemo.HealthCentral.Abstractions;

namespace FusionDemo.HealthCentral.Services
{
    public class TimeService : ITimeService
    {
        [ComputeMethod(AutoInvalidationDelay = 5, MinCacheDuration = 5)]
        public virtual Task<DateTime> GetTimeAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(DateTime.Now);
        }

    }
}
