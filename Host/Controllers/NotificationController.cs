using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController, JsonifyErrors]
    public class NotificationController: ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService) => this.notificationService = notificationService;


        [HttpPost("add")]
        public async Task<IActionResult> AddNotification(string message)
        {
            await notificationService.AddNotification(message);
            return new OkResult();
        }

        [HttpGet("get"), Publish]
        public async Task<AppNotification> GetNotification(CancellationToken cancellationToken)
        {
            return await notificationService.GetNotification(cancellationToken);
        }
    }
}
