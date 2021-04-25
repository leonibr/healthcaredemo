using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Domain
{
    public record Patient
    {
        public Guid PatientId { get; set; }
        public string Name { get; set; }

        public GenderType Gender { get; set; }

        //for the sake of simplicity
        public TimeSpan Age => DateTime.Now.Subtract(DOB);
        public TimeSpan TimeInLine => DateTime.Now.Subtract(WaitingSince);
        public DateTime DOB { get; set; }
        public DateTime WaitingSince { get; set; } = DateTime.Now;       

    }


}
