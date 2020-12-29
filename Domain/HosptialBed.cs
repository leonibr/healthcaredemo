
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Domain
{
    public class HospitalBed
    {
        public int HospitalBedId { get; set; }

        public bool IsFree => Patient == null;

        public Patient Patient { get; set; }

        public TimeSpan OccupationDuration => OccupiedSince.HasValue? DateTime.Now.Subtract(OccupiedSince.Value) : TimeSpan.FromSeconds(0);

        public DateTime? OccupiedSince { get; set; }
    }
}
