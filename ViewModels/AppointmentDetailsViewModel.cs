using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public string AppointmentId { get; set; }
        public long TimeSlotId { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public int AppointmentStatus { get; set; }
        public decimal Fee { get; set; }
        public string Date { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public bool IsSlotAvailable { get; set; }
    }

    public class AdminAppointmentDetailsViewModel
    {
        public string AppointmentId { get; set; }
        public long TimeSlotId { get; set; }
        public string DoctorId { get; set; }
        public string DoctorFirstName {get; set;}
        public string DoctorLastName {get; set;}
        public string DoctorEmail {get; set;}
        public string DoctorPhoneNumber {get; set;}
        public string PatientId { get; set; }
        public string PatientFirstName {get; set;}
        public string PatientLastName {get; set;}
        public string PatientEmail {get; set;}
        public string PatientPhoneNumber {get; set;}
        public int AppointmentStatus { get; set; }
        public string AppointmentStatusDisplay { get; set; }
        public decimal Fee { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public string AppointmentDate { get; set; }
        public int appointmentTypeId { get; set; }
        public string appointmentTypeName { get; set; }
    }
}
