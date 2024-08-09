using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AppointmentDetailsForNotificationViewModel
    {
        public string AppointmentId { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public int AppointmentStatus { get; set; }
        public string Date { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
       
    }
}
