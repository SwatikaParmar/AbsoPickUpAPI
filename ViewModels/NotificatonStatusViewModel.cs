using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class NotificatonStatusViewModel
    {
        public bool? EmailNotificationStatus { get; set; }
        public bool? SMSNotificationStatus { get; set; }
    }
}
