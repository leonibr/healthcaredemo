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
    public class PatientController : ControllerBase
    {
        private readonly IPatientService patientService;

        public PatientController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        [HttpPost("add-patient-to-waiting-list")]
        public async Task<IActionResult> AddPatientToWaitingList(CancellationToken cancellationToken = default)
        {
            await patientService.AddPatientToWaitingList(cancellationToken);

            return new OkResult();
        }

        [HttpPost("discharge-patient")]
        public Task<bool> DischargePatientFromBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default)
        {
           return patientService.DischargePatientFromBed(patientId: patientId, hospitalBedId: hospitalBedId, careUnitId: careUnitId, cancellationToken);
        }

        [HttpGet("available-units"), Publish]
        public async Task<IEnumerable<CareUnit>> GetAvailableUnits(CancellationToken cancellationToken)
        {
            return await patientService.GetAvailableUnits(cancellationToken);
        }

        [HttpGet("patients-waiting-list"), Publish]
        public async Task<IEnumerable<Patient>> GetPatientWaitingList(CancellationToken cancellationToken = default)
        {
            return await patientService.GetPatientWaitingList(cancellationToken);
        }


        [HttpPost("put-on-bed")]
        public Task<bool> PutPatientOnBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default)
        {
            return patientService.PutPatientOnBed(patientId: patientId, hospitalBedId: hospitalBedId, careUnitId: careUnitId, cancellationToken);
        }
    }
}
