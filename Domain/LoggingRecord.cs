using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Domain
{
    public record LoggingRecord
    {
        public DateTime When { get; set; }

        public string Browser{ get; set; }

        public string Path { get; set; }
        /// <summary>
        /// Http Method used.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Duration of request in ms
        /// </summary>
        public double Duration { get; set; }
        public int Status { get; set; }

        public override string ToString()
        {
            return $"Request {Method} {Path}\nDuration: {Duration:000.00}ms\t Status: {Status}"; 
        }
    }
}
