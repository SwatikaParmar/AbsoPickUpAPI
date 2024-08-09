using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientPastAppointmentViewModel
    {
        public string AppointmentId { get; set; }
        public long TimeSlotId { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public string Dr_FirstName { get; set; }
        public string Dr_LastName { get; set; }
        public string Dr_ProfilePic { get; set; }
        public List<SpecialityInfoViewModel> DoctorSpecialities { get; set; }
        public string Date { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public int AppointmentStatus { get; set; }
        public int PaymentStatus { get; set; }
        public int Rating { get; set; }
    }
}
