using System;
using System.Threading;
using System.Threading.Tasks;
using RestEase;
using Stl.Fusion.Client;
using FusionDemo.HealthCentral.Abstractions;
using System.Collections.Generic;
using FusionDemo.HealthCentral.Domain;
using Stl.Fusion.Authentication;
using System.Net.Http;

namespace FusionDemo.HealthCentral.UI.Services
{
    [RegisterRestEaseReplicaService(typeof(ITimeService), Scope = Program.ClientSideScope)]
    [BasePath("time")]
    public interface ITimeClient
    {
        [Get("get")]
        Task<DateTime> GetTimeAsync(CancellationToken cancellationToken = default);
    }


    [RegisterRestEaseReplicaService(typeof(IPatientService), Scope = Program.ClientSideScope)]
    [BasePath("patient")]
    public interface IPatientClient 
    {
      

        [Get("available-units")]
        Task<IEnumerable<CareUnit>> GetAvailableUnits(CancellationToken cancellationToken);

        [Get("patients-waiting-list")]
        Task<IEnumerable<Patient>> GetPatientsWaitingList(CancellationToken cancellationToken);

       
        [Post("add-patient-to-waiting-list")]
        Task AddPatientToWaitingList(CancellationToken cancellationToken = default);
        [Post("clear-waiting-list")]
        Task ClearWaitingList(CancellationToken cancellationToken = default);

        [Post("empty-hospital-beds")]
        Task EmptyHospitalBeds(CancellationToken cancellationToken = default);

        [Post("put-on-bed")]
        Task<bool> PutPatientOnBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default);

        [Post("discharge-patient")]
        Task<bool> DischargePatientFromBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default);


    }


    [RegisterRestEaseReplicaService(typeof(IPainelComposerService), Scope = Program.ClientSideScope)]
    [BasePath("painel-composer")]
    public interface IPainelComposerClient
    {
        [Get("get")]
        Task<PainelComposedValue> GetComposedValueAsync(
            string? parameter,
            CancellationToken cancellationToken = default);
    }

    [RegisterRestEaseReplicaService(typeof(INotificationService), Scope = Program.ClientSideScope)]
    [BasePath("notification")]
    public interface INotificationClient
    {
        [Get("get")]
        Task<AppNotification> GetNotification(CancellationToken cancellationToken);

        [Post("add")]
        Task AddNotification(string message);
    }

    [RegisterRestEaseReplicaService(typeof(IRequestLoggingService), Scope = Program.ClientSideScope)]
    [BasePath("logging")]
    public interface IRequestLoggingClient
    {
        [Post]
        Task<HttpResponseMessage> AddLoggingRecord([Body(serializationMethod: BodySerializationMethod.Serialized)] LoggingRecord loggingRecord);

        [Get()]
        Task<IEnumerable<LoggingRecord>> GetLatest(CancellationToken cancellationtoken);
    }
}
