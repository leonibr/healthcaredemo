using System;
using System.Threading;
using System.Threading.Tasks;
using RestEase;
using Stl.Fusion.Client;
using FusionDemo.HealthCentral.Abstractions;
using System.Collections.Generic;
using FusionDemo.HealthCentral.Domain;
using Stl.Fusion.Authentication;

namespace FusionDemo.HealthCentral.UI.Services
{
    [RestEaseReplicaService(typeof(ITimeService), Scope = Program.ClientSideScope)]
    [BasePath("time")]
    public interface ITimeClient
    {
        [Get("get")]
        Task<DateTime> GetTimeAsync(CancellationToken cancellationToken = default);
    }


    [RestEaseReplicaService(typeof(IPatientService), Scope = Program.ClientSideScope)]
    [BasePath("patient")]
    public interface IPatientClient 
    {
      

        [Get("available-units")]
        Task<IEnumerable<CareUnit>> GetAvailableUnits(CancellationToken cancellationToken);

        [Get("patients-waiting-list")]
        Task<IEnumerable<Patient>> GetPatientsWaitingList(CancellationToken cancellationToken);

       
        [Post("add-patient-to-waiting-list")]
        Task AddPatientToWaitingList(CancellationToken cancellationToken = default);

        [Post("put-on-bed")]
        Task<bool> PutPatientOnBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default);

        [Post("discharge-patient")]
        Task<bool> DischargePatientFromBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default);


    }


    [RestEaseReplicaService(typeof(IPainelComposerService), Scope = Program.ClientSideScope)]
    [BasePath("painel-composer")]
    public interface IPainelComposerClient
    {
        [Get("get")]
        Task<PainelComposedValue> GetComposedValueAsync(
            string? parameter,
            CancellationToken cancellationToken = default);
    }

    [RestEaseReplicaService(typeof(INotificationService), Scope = Program.ClientSideScope)]
    [BasePath("notification")]
    public interface INotificationClient
    {
        [Get("get")]
        Task<AppNotification> GetNotification(CancellationToken cancellationToken);

        [Post("add")]
        Task AddNotification(string message);
    }

}
