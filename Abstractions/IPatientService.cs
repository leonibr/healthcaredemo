using FusionDemo.HealthCentral.Domain;
using Stl.Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Abstractions
{
    public interface IPatientService
    {
        Task AddPatientToWaitingList(CancellationToken cancellationToken = default);
        Task<bool> DischargePatientFromBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default);
        [ComputeMethod]
        Task<IEnumerable<Domain.CareUnit>> GetAvailableUnits(CancellationToken cancellationToken = default);
        [ComputeMethod]
        Task<IEnumerable<Domain.Patient>> GetPatientWaitingList(CancellationToken cancellationToken = default);
        Task<bool> PutPatientOnBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default);
    }
}
