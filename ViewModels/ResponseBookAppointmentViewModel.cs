using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseBookAppointmentViewModel
    {
        public string AppointmentId { get; set; }
        public bool Status { get; set; }
        public string Date { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public int ResponseValue { get; set; }
    }
}
