using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorUpcomingAppointmentsViewModel
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientProfilePic { get; set; }
        public string AppointmentId { get; set; }
        public int AppointmentStatus { get; set; }
        public long TimeSlotId { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public string Date { get; set; }
    }
}
