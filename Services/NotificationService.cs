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
    public class NotificationService : INotificationService
    {
        private readonly ConcurrentQueue<AppNotification> Queue = new();

        [ComputeMethod(MinCacheDuration = 60, AutoInvalidationDelay = 1)]
        public virtual Task<AppNotification> GetNotification(CancellationToken  cancellationToken = default)
        {
            var hasNotification = Queue.TryDequeue(out AppNotification? notification);
            if (hasNotification && notification != null)
            {
                return Task.FromResult(notification);
            }
            AppNotification empty = null!;
            return Task.FromResult(empty);

        }

        public async Task AddNotification(string message)
        {
            var appNot = AppNotification.Create();
            appNot.AddMessage(message);
            Queue.Enqueue(appNot);
            using (Computed.Invalidate())
                await GetNotification();
        }
    }
}
