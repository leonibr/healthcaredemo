﻿using FusionDemo.HealthCentral.Domain;
using Stl.Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Abstractions
{
    public interface INotificationService
    {
        Task AddNotification(string message);
        [ComputeMethod(MinCacheDuration = 5, AutoInvalidationDelay = 1)]
        Task<AppNotification> GetNotification(CancellationToken cancellationtoken);
    }
}
