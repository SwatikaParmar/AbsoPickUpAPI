using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AppointmentListViewModel
    {
        public string appointmentId { get; set; }
        public string appointmentDate { get; set; }
        public string appointmentStartTime { get; set; }
        public string appointmentEndTime { get; set; }
        public int? appointmentstatus { get; set; }
        public string appointmentstatusDisplay { get; set; }
        public decimal appointmentFee { get; set; }
        public string doctorName { get; set; }
        public string doctorEmail { get; set; }
        public string doctorPhone { get; set; }
        public string patientFirstName { get; set; }
        public string patientLastName { get; set; }
        public string patientPhoneNumber { get; set; }
        public int appointmentTypeId { get; set; }
        public string appointmentTypeName { get; set; }

    }
}