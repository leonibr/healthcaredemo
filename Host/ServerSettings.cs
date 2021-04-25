using System;
using System.Text;
using Stl.DependencyInjection;

namespace FusionDemo.HealthCentral.Host
{
    [Settings("Server")]
    public class ServerSettings
    {
        public string PublisherId { get; set; } = "p";
      
    }
}
