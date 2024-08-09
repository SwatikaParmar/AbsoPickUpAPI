using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class SendMessageViewModel
    {
        public string ToId { get; set; }
        public string AppointmentId { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
    }
}
