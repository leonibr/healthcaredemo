using Bogus;
using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Stl.Fusion;
using Stl.Async;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Stl.RegisterAttributes;

namespace FusionDemo.HealthCentral.Services
{
    //[RegisterService(typeof(IPatientService))]
    public class PatientService : IPatientService
    {

        private ConcurrentDictionary<Guid, Patient> waitingList = new ConcurrentDictionary<Guid, Patient>();
        private ConcurrentDictionary<int, CareUnit> careUnits = new ConcurrentDictionary<int, CareUnit>();

        private IList<string> notifications = new List<string>();
        private readonly INotificationService notificationService;

        private readonly Faker<Patient> faker = new();


        public PatientService(INotificationService notificationService)
        {
            // one time setup

            SetUpFakerPatient(faker);

            var unit1 = new CareUnit()
            {
                CareUnitId = 1,
                Name = "Better Hope Hospital",
                HospitalBeds = new List<HospitalBed>()
                {
                    new HospitalBed()
                    {
                        HospitalBedId = 1,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 2,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 3,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 4,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 5,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 6,
                        Patient = null,
                        OccupiedSince = null
                    }
                }
            };

            var unit2 = new CareUnit()
            {
                CareUnitId = 2,
                Name = "All The Missing Ones Hospital",
                HospitalBeds = new List<HospitalBed>()
                {
                    new HospitalBed()
                    {
                        HospitalBedId = 7,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 8,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 9,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 10,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 11,
                        Patient = null,
                        OccupiedSince = null
                    },
                      new HospitalBed()
                    {
                        HospitalBedId = 12,
                        Patient = null,
                        OccupiedSince = null
                    }
                }
            };

            careUnits.TryAdd(unit1.CareUnitId, unit1);
            careUnits.TryAdd(unit2.CareUnitId, unit2);
            this.notificationService = notificationService;
        }

        private void SetUpFakerPatient(Faker<Patient> faker)
        {         
            faker
                .RuleFor(c => c.Gender, (f) => f.PickRandom<GenderType>())
                .RuleFor(c => c.Name, (f, u) => f.Name.FullName(u.Gender == GenderType.Male ?
                    Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female))
                .RuleFor(c => c.DOB, (f, u) => f.Date.Between(DateTime.Now.AddYears(-35), DateTime.Now.AddYears(-95)))
                .RuleFor(c => c.PatientId, f => Guid.NewGuid());
        }

        [ComputeMethod]
        public virtual Task<IEnumerable<CareUnit>> GetAvailableUnits(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(careUnits.Values.OrderByDescending(c => c.FreeBeds).ThenBy(c => c.Name).ToList().AsEnumerable());
        }

        [ComputeMethod]
        public virtual Task<IEnumerable<Patient>> GetPatientWaitingList(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(waitingList.Values.AsEnumerable());
        }


        public Task ClearWaitingList(CancellationToken cancellation = default)
        {
            notificationService.AddNotification($"Some user has just reset the waiting list");
            waitingList.Clear();
            using (Computed.Invalidate())
                GetPatientWaitingList();
            return Task.CompletedTask;
        }   
        
        public Task EmptyHospitalBeds(CancellationToken cancellation = default)
        {
            notificationService.AddNotification($"Some user has just reset the Hospital beds");
            careUnits.Values.ToList().ForEach(c =>
            {
                c.HospitalBeds.ToList().ForEach(bed =>
                {
                    bed.Patient = null!;
                    bed.OccupiedSince = null!;
                });
            });

            using (Computed.Invalidate())
                GetAvailableUnits(cancellation).Ignore();
            return Task.CompletedTask;
        }


        // public void AddPatientToWaitingList(Patient patient, CancellationToken cancellationToken = default)
        public Task AddPatientToWaitingList(CancellationToken cancellationToken = default)
        {
            if (waitingList.Count >= 50)
            {
                notificationService.AddNotification($"Waiting List limit reached!");
                return Task.CompletedTask;
            }

         

            Patient patient = faker.Generate(1).First();


            waitingList.TryAdd(patient.PatientId, patient);
            using (Computed.Invalidate())
                GetPatientWaitingList();
            notificationService.AddNotification($"New Patient to waiting list\n{patient.Name}");
            return Task.CompletedTask;

        }

        public Task<bool> PutPatientOnBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default)
        {

            var unitExists = careUnits.TryGetValue(careUnitId, out CareUnit careUnit);
            var patientExists = waitingList.TryGetValue(patientId, out Patient patient);

            if (unitExists && patientExists)
            {
                var bed = careUnit.HospitalBeds.Where(b => b.HospitalBedId == hospitalBedId).FirstOrDefault();
                if (bed != null && bed.IsFree)
                {
                    bed.Patient = patient;
                    bed.OccupiedSince = DateTime.Now;

                    using (Computed.Invalidate())
                        GetAvailableUnits(cancellationToken).Ignore();

                    var removed = waitingList.TryRemove(patient.PatientId, out Patient removedPatient);
                    if (removed)
                    {
                        using (Computed.Invalidate())
                            GetPatientWaitingList(cancellationToken).Ignore();
                    }
                    return Task.FromResult(removed);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            return Task.FromResult(false);




        }

        public Task<bool> DischargePatientFromBed(Guid patientId, int hospitalBedId, int careUnitId, CancellationToken cancellationToken = default)
        {

            var unitExists = careUnits.TryGetValue(careUnitId, out CareUnit careUnit);
            var bed = careUnit?.HospitalBeds.Where(b => b.HospitalBedId == hospitalBedId).FirstOrDefault();

            if (unitExists
                && bed != null
                && !bed.IsFree &&
                bed.Patient.PatientId == patientId)
            {
                bed.Patient = null!;
                bed.OccupiedSince = null!;

                using (Computed.Invalidate())
                    GetAvailableUnits(cancellationToken).Ignore();


                return Task.FromResult(true);

            }
            return Task.FromResult(false);

        }


    }
}
