using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Services
{
    using Domain;
    using Abstractions;
    using System.Collections.Concurrent;
    using Stl.Fusion;
    using System.Threading;

    [ComputeService(typeof(INotificationService))]
    public class NotificationService : INotificationService
    {
        private ConcurrentQueue<AppNotification> queue = new ConcurrentQueue<AppNotification>();

        [ComputeMethod(KeepAliveTime = 1, AutoInvalidateTime = 1)]
        public virtual Task<AppNotification> GetNotification(CancellationToken  cancellationToken = default)
        {
            var hasNotification = queue.TryDequeue(out AppNotification notification);
            if (hasNotification && notification != null)
            {
                return Task.FromResult(notification);
            }
            AppNotification empty = null!;
            return Task.FromResult(empty);

        }

        public Task AddNotification(string message)
        {
            var appNot = AppNotification.Create();
            appNot.AddMessage(message);
            queue.Enqueue(appNot);
            Computed.Invalidate(() => GetNotification());
            return Task.CompletedTask;
        }
    }
}
