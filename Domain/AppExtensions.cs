using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Domain
{
    public static class AppExtensions
    {
        public static string ToAgeString(this TimeSpan timeSpan)
        {

            var years = timeSpan.Days / 365;
            var months = (timeSpan.Days % 365) / 30;
            return $"{years} yrs, {months} months";

        }

        public static string ToTimeInLineString(this TimeSpan timeSpan)
        {

            var days = timeSpan.Days > 0 ? $"{timeSpan.Days} days, " : "";
            var hours = timeSpan.Hours > 0 ? $"{timeSpan.Hours} hours, " : "";
            var minutes = timeSpan.Minutes > 0 ? $"{timeSpan.Minutes} minutes, " : "";
            var seconds = $"{timeSpan.Seconds} seconds.";
            return $"{days}{hours}{minutes}{seconds}";
        }

        
    }
}
