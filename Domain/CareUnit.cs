using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Domain
{
    public class CareUnit
    {
        public int CareUnitId { get; set; }

        public string Name { get; set; }

        public int Capacity => HospitalBeds.Count();

        public int FreeBeds => HospitalBeds.Where(h => h.IsFree == true).Count();

        public ICollection<HospitalBed> HospitalBeds { get; set; }
    }
}
