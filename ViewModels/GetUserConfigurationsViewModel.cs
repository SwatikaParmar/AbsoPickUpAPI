using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetUserConfigurationsViewModel
    {
            public long Id { get; set; }
            public string UserId { get; set; }
            public bool PushNotificationStatus { get; set; }
            public bool EmailNotificationStatus { get; set; }
            public bool SMSNotificationStatus { get; set; }
        
    }
}
