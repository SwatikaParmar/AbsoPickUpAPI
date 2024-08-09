using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientMedicalHistoryViewModel
    {
        [Required]
        public string TreatmentName { get; set; }
        [Required]
        public string DoctorName { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string Description { get; set; }

        public string patientId {get; set;}
    }
}
