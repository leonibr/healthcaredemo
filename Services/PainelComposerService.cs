using FusionDemo.HealthCentral.Abstractions;
using Microsoft.Extensions.Logging;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Services
{
    [ComputeService(typeof(IPainelComposerService))]
    public class PainelComposerService : IPainelComposerService
    {
        private readonly ITimeService timeService;
        private readonly IPatientService patientService;
        private readonly ILogger<PainelComposerService> logger;

        public PainelComposerService(ITimeService timeService, IPatientService patientService, ILogger<PainelComposerService> logger)
        {
            this.timeService = timeService;
            this.patientService = patientService;
            this.logger = logger;
            logger.LogInformation("PainelComposedService Started");
        }
        public virtual async Task<PainelComposedValue> GetComposedValueAsync(string parameter, CancellationToken cancellationToken = default)
        {
            var timeValueTask = timeService.GetTimeAsync(cancellationToken);
            var availableUnitTask = patientService.GetAvailableUnits(cancellationToken);
            var waitingListTask = patientService.GetPatientWaitingList(cancellationToken);
           

            await Task.WhenAll(new Task[]
            {
                timeValueTask,
                availableUnitTask,
                waitingListTask,
            });

            return new PainelComposedValue()
            {
                ServerTime = timeValueTask.Result,
                AvailableUnits = availableUnitTask.Result,
                PatientsWaitingList = waitingListTask.Result,
            };
        }
    }
}
