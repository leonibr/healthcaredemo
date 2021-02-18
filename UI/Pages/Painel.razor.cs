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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.UI.Pages
{
    public class PainelPage : LiveComponentBase<PainelComposedValue>
    {
        [Inject] IPainelComposerService PainelComposerService { get; set; }
        [Inject] IPatientService PatientService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] ILogger<Painel> Logger { get; set; }
 
        [Inject] IDialogService Dialog { get; set; }
        protected string draggingCss { get; set; } = "";
        protected Patient DragPatient { get; set; } = null!;


        protected IList<Patient> patients { get; set; } = new List<Patient>();
        protected IList<CareUnit> units { get; set; } = new List<CareUnit>();

        protected IList<Patient> FiltredPatientList  => filterPatient(patients);

        protected string[] labels = { "Free", "Occuppied" };
        public string SelectedFilterPatient { get; set; } = "2";
        public CareUnit SelectedCareUnit { get; set; } = null!;

        public FilterByPatientOption SelectedOrderBy { get; set; } = FilterByPatientOption.ArrivalDescending;
        protected Func<FilterByPatientOption, string> FilterConverter = option => option.ToString();
        public enum FilterByPatientOption
        {
            AgeDescending = 1,
            ArrivalDescending = 2,
            AgeAscending = 3,
            ArrivalAscending = 4,
        }

        protected void HandleDragStart(Patient dragPatient)
        {
            draggingCss = "dragging-start";
            InvokeAsync(() => DragPatient = dragPatient);
            Console.WriteLine($"HandleDragStart Patient {DragPatient.Name}");

        }


        protected void HandleDragEnd(DragEventArgs e)
        {
            Console.WriteLine("HandleDragEnd");
            draggingCss = "";
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
                case FilterByPatientOption.AgeDescending:
                    filtredPatients = statePatients.OrderByDescending(c => c.Age).ToList();
                    break;
                case FilterByPatientOption.ArrivalDescending:
                    filtredPatients = statePatients.OrderByDescending(c => c.TimeInLine).ToList();
                    break;
                case FilterByPatientOption.AgeAscending:
                    filtredPatients = statePatients.OrderBy(c => c.Age).ToList();
                    break;
                case FilterByPatientOption.ArrivalAscending:
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

        protected override void ConfigureState(LiveState<PainelComposedValue>.Options options)
        {
            options.WithUpdateDelayer(0.5);


        }
        protected void SelectUnit(CareUnit unit)
        {

            if (SelectedCareUnit != null && unit != null && SelectedCareUnit?.CareUnitId == unit.CareUnitId)
            {
                SelectedCareUnit = null!;
            }
            else
            {
                SelectedCareUnit = unit;
            }
        }




        protected override async Task<PainelComposedValue> ComputeStateAsync(CancellationToken cancellationToken)

        {
            var parameter = "PainelSample-";
            var composedValue = await PainelComposerService.GetComposedValueAsync(parameter, cancellationToken);
            Console.WriteLine("ComputeStateAsync " + JsonConvert.SerializeObject(composedValue));
            if (SelectedCareUnit != null)
            {
                SelectedCareUnit = composedValue.AvailableUnits.Where(c => c.CareUnitId == SelectedCareUnit.CareUnitId).FirstOrDefault()!;
            }
            return composedValue;
        }

        protected string formatAge(TimeSpan timeSpan)
        {

            var years = timeSpan.Days / 365;
            var months = (timeSpan.Days % 365) / 30;
            return $"{years} yrs, {months} months";

        }

        protected string formatTimeInLine(TimeSpan timeSpan)
        {

            var days = timeSpan.Days > 0 ? $"{timeSpan.Days} days, " : "";
            var hours = timeSpan.Hours > 0 ? $"{timeSpan.Hours} hours, " : "";
            var minutes = timeSpan.Minutes > 0 ? $"{timeSpan.Minutes} minutes, " : "";
            var seconds = $"{timeSpan.Seconds} seconds.";
            return $"{days}{hours}{minutes}{seconds}";
        }

        public async Task PutPatient(Patient patient, HospitalBed bed = null)
        {
            var parameters = new DialogParameters();
            parameters.Add("patient", patient);
            parameters.Add("units", State.LastValue.AvailableUnits);
            parameters.Add("preSelectedBed", bed);

            var dialog = Dialog.Show<PainelDialogs.PutPatientOnBed>("Put Patient", parameters, new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true });
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
               var data = ((Guid PatientId, int CareUnitId, int HospitalBedId))result.Data;

               var ok = await PatientService.PutPatientOnBed(data.PatientId, data.HospitalBedId, data.CareUnitId);
                if (!ok)
                    Snackbar.Add($"Could not put {patient?.Name} on bed {bed?.HospitalBedId}", Severity.Error);


            }
        }

        protected async Task DischargePatient(Patient patient, HospitalBed bed)
        {
            var parameters = new DialogParameters();
            parameters.Add("patient", patient);
            parameters.Add("units", State.LastValue.AvailableUnits);
            parameters.Add("preSelectedBed", bed);

            var dialog = Dialog.Show<PainelDialogs.DischargePatient>($"Discharge Patient: {patient.Name}", parameters, new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true });
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                var data = ((Guid PatientId, int CareUnitId, int HospitalBedId))result.Data;

                var ok = await PatientService.DischargePatientFromBed(
                    patientId: data.PatientId,
                    hospitalBedId: data.HospitalBedId,
                    careUnitId: data.CareUnitId);
                if (!ok)
                    Snackbar.Add($"Could not put {patient?.Name} on bed {bed?.HospitalBedId}", Severity.Error);


            }
        }


        protected void Refresh()
        {
            State.Invalidate();
            State.CancelUpdateDelay();
        }

        protected async  Task AddToWaitingList()
        {
            await PatientService.AddPatientToWaitingList();
        }
    }
}
