using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionDemo.HealthCentral.Domain
{
    public class AppNotification
    {
        public Guid AppNotificationId { get; set; } = Guid.NewGuid();

        public IList<string> Messages { get; set; } = new List<string>();

        public DateTime Date { get; set; } = DateTime.Now;



        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public static AppNotification Create()
        {
            return new AppNotification();
        }

    }
}
