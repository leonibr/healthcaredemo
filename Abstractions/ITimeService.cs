using System;
using System.Threading;
using System.Threading.Tasks;
using Stl.Fusion;

namespace FusionDemo.HealthCentral.Abstractions
{
    public interface ITimeService
    {
        [ComputeMethod]
        Task<DateTime> GetTimeAsync(CancellationToken cancellationToken = default);
    }
}
