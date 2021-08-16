using System;
using System.Threading;
using System.Threading.Tasks;
using Stl.Fusion;
using FusionDemo.HealthCentral.Abstractions;

namespace FusionDemo.HealthCentral.Services
{
    //[RegisterService(typeof(ITimeService))]
    public class TimeService : ITimeService
    {
        [ComputeMethod(AutoInvalidateTime = 5, KeepAliveTime =60)]
        public virtual Task<DateTime> GetTimeAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(DateTime.Now);
        }



    }
}
