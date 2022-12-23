using FusionDemo.HealthCentral.Abstractions;
using FusionDemo.HealthCentral.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Dialog;
using Newtonsoft.Json;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.Blazor;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.UI.Pages
{
    public  class PainelPage : ComputedStateComponent<PainelComposedValue>
    {
        [Inject] [AllowNull, NotNull] IPainelComposerService PainelComposerService { get; set; }
        [Inject] [AllowNull, NotNull] IPatientService PatientService { get; set; }
        [Inject] [AllowNull, NotNull] ISnackbar Snackbar { get; set; }
        [Inject] [AllowNull, NotNull] ILogger<Painel> Logger { get; set; }
        [Inject] [AllowNull, NotNull] IRequestLoggingService RequestLogging { get; set; } 
        [Inject] [AllowNull, NotNull] IDialogService Dialog { get; set; }
        protected string DraggingCss { get; set; } = "";

        [AllowNull]
        protected Patient DragPatient { get; set; } = null;


        protected IList<Patient> Patients { get; set; } = new List<Patient>();
        protected IList<CareUnit> Units { get; set; } = new List<CareUnit>();

        protected IList<Patient> OrderedPatientList  => filterPatient(Patients);
               
        public string SelectedFilterPatient { get; set; } = "2";
        public CareUnit SelectedCareUnit { get; set; } = null!;

        public OrderByPatientOption SelectedOrderBy { get; set; } = OrderByPatientOption.ArrivalDescending;
        protected Func<OrderByPatientOption, string> FilterConverter = option => option.ToString();


        public enum OrderByPatientOption
        {
            AgeDescending = 1,
            ArrivalDescending = 2,
            AgeAscending = 3,
            ArrivalAscending = 4,
        }

        protected void HandleDragStart(Patient dragPatient)
        {
            DraggingCss = "dragging-start";
            InvokeAsync(() => DragPatient = dragPatient);
            Console.WriteLine($"HandleDragStart Patient {DragPatient.Name}");

        }


        protected void HandleDragEnd(DragEventArgs e)
        {
            Console.WriteLine("HandleDragEnd");
            DraggingCss = "";
        }

        public async Task HandleDrop(HospitalBed bed)
        {
            Console.WriteLine($"HandleDrop Patient {DragPatient?.Name} to bed code: {bed.HospitalBedId} bed is free: {bed.IsFree}");

            // dropClass = "";
            if (DragPatient == null)
            {
                Snackbar.Add("Select a Patient first!", Severity.Warning, (o) => o.ShowTransitionDuration = 3000);
                return;
            };

            if (!bed.IsFree)
            {
                Snackbar.Add($"Already Occupied by {bed.Patient?.Name}");
                return;
            }


            await PutPatient(DragPatient, bed);
            DragPatient = null;


        }

        protected List<Patient> filterPatient(IEnumerable<Patient> statePatients)
        {
            if (SelectedOrderBy == null)
            {
                return statePatients.ToList();
            }
            List<Patient> filtredPatients;
            var op = SelectedOrderBy; // (FilterByPatientOption)Enum.Parse(typeof(FilterByPatientOption), SelectedFilterPatient);

            switch (op)
            {
                case OrderByPatientOption.AgeDescending:
                    filtredPatients = statePatients.OrderByDescending(c => c.Age).ToList();
                    break;
                case OrderByPatientOption.ArrivalDescending:
                    filtredPatients = statePatients.OrderByDescending(c => c.TimeInLine).ToList();
                    break;
                case OrderByPatientOption.AgeAscending:
                    filtredPatients = statePatients.OrderBy(c => c.Age).ToList();
                    break;
                case OrderByPatientOption.ArrivalAscending:
                    filtredPatients = statePatients.OrderBy(c => c.TimeInLine).ToList();
                    break;
                default:
                    filtredPatients = statePatients.ToList();
                    break;
            }
            return filtredPatients;
        }

        protected void OnPatientFilterChanged(string value)
        {
            SelectedFilterPatient = value;
            StateHasChanged();
        }


        protected override void OnInitialized()
        {
            StateHasChangedTriggers = StateEventKind.All;
            base.OnInitialized();
        }

        protected override ComputedState<PainelComposedValue>.Options GetStateOptions()
        {

            return base.GetStateOptions();
        }

        //protected override void ComputeState(ComputedState<PainelComposedValue>.Options options)
        //{
        // //    options.UpdateDelayer = new UpdateDelayer();
        //}
        protected void SelectUnit([AllowNull, MaybeNull] CareUnit unit)
        {

            if (SelectedCareUnit != null && unit != null && SelectedCareUnit?.CareUnitId == unit.CareUnitId)
            {
                SelectedCareUnit = null!;
            }
            else
            {
                SelectedCareUnit = unit;
            }
            StateHasChanged();
        }




        protected override async Task<PainelComposedValue> ComputeState(CancellationToken cancellationToken)

        {
            var parameter = "PainelSample-";
            var composedValue = await PainelComposerService.GetComposedValueAsync(parameter, cancellationToken);
          
            if (SelectedCareUnit != null)
            {
                SelectedCareUnit = composedValue.AvailableUnits.Where(c => c.CareUnitId == SelectedCareUnit.CareUnitId).FirstOrDefault()!;
            }
            return composedValue;
        }



        public async Task PutPatient(Patient patient, [AllowNull, MaybeNull] HospitalBed bed = null)
        {
            var parameters = new DialogParameters();
            parameters.Add("patient", patient);
            parameters.Add("units", State.Value.AvailableUnits);
            parameters.Add("preSelectedBed", bed);

            var dialog = Dialog.Show<PainelDialogs.PutPatientOnBed>("Put Patient", parameters, new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true });
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
               var data = ((Guid PatientId, int CareUnitId, int HospitalBedId))result.Data;

               var ok = await PatientService.PutPatientOnBed(data.PatientId, data.HospitalBedId, data.CareUnitId);
                if (!ok)
                {
                    Snackbar.Add($"Could not put {patient?.Name} on bed {bed?.HospitalBedId}", Severity.Error);                  

                }


            }
        }

        protected async Task DischargePatient(HospitalBed bed)
        {
            var parameters = new DialogParameters();
            var patient = bed.Patient;
            parameters.Add("patient", patient);
            parameters.Add("units", State.Value.AvailableUnits);
            parameters.Add("preSelectedBed", bed);

            var dialog = await Dialog.ShowAsync<PainelDialogs.DischargePatient>($"Discharge Patient: {patient.Name}", parameters, new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true });
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                var data = ((Guid PatientId, int CareUnitId, int HospitalBedId))result.Data;

                var ok = await PatientService.DischargePatientFromBed(
                    patientId: data.PatientId,
                    hospitalBedId: data.HospitalBedId,
                    careUnitId: data.CareUnitId);
                var currentUnit = Units.Where(b => b.HospitalBeds.Any(b => b.HospitalBedId == bed?.HospitalBedId)).Select(u => u.Name).FirstOrDefault();
                if (ok)
                    Snackbar.Add($"Patient: {patient?.Name} left bed {bed?.HospitalBedId} from {currentUnit}", Severity.Success);
                else
                    Snackbar.Add($"Could not discharge {patient?.Name} on bed {bed?.HospitalBedId}", Severity.Error);


            }
        }


        protected void Refresh()
        {
            State.Invalidate();
            //State.();
        }

        protected async  Task AddToWaitingList()
        {
            await PatientService.AddPatientToWaitingList();
        }

        protected async Task ClearWaitingList()
        {
            await PatientService.ClearWaitingList();
        }  
        protected async Task EmptyHospitalBeds()
        {
            await PatientService.EmptyHospitalBeds();
        }
    }
}
