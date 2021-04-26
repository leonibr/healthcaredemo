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
    public class LoggingController: ControllerBase
    {
        private readonly IRequestLoggingService loggingService;

        public LoggingController(IRequestLoggingService logginService) => this.loggingService = logginService;

        [HttpPost]
        public async Task<IActionResult> AddLoggingRecord(LoggingRecord loggingRecord, CancellationToken cancellationToken)
        {
            await loggingService.AddLoggingRecord(loggingRecord);
            return new OkResult();
        }


        [HttpGet, Publish]
        public async Task<IEnumerable<LoggingRecord>> GetLatest(CancellationToken cancellationtoken)
        {
            return await loggingService.GetLatest(cancellationtoken);
        }

     
    }
}
