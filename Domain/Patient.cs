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

        public string Initials
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Name))
                    return string.Empty;

                var arr = Name.Split(' ');
                if (arr.Length == 1)
                {
                    return arr[0].ToUpperInvariant().First().ToString();
                }

                if (arr.Length >= 2)
                {
                    var first = arr[0].ToUpperInvariant().First().ToString();
                    var second = arr[1].ToUpperInvariant().First().ToString();
                    return first + second;
                }

                return string.Empty;


            }
        }

    }


}
