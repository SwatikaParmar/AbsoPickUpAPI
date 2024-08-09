using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorPatientUpcomingAppointmentViewModel
    {
        public string AppointmentId { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
       
        public string Date { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public int EstimatedTimeInMinutes { get; set; }
    }
}
