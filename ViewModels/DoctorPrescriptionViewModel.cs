using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PrescriptionsViewModel
    {
        [Required]
        public string PatientId { get; set; }
        [Required]
        public string AppointmentId { get; set; }
        [Required]
        public string MedicareNumber { get; set; }
        [Required]
        public string ReferenceNumber { get; set; }
        [Required]
        public string EntitlementNumber { get; set; }
        [Required]
        public bool ISPBSSafetyEntitlementCardHolder { get; set; }
        [Required]
        public bool IsPBSSafetyConcessionCardHolder { get; set; }
        [Required]
        public bool IsPBSPrescriptionFromStateManager { get; set; }
        [Required]
        public bool IsRBPSPrescription { get; set; }
        [Required]
        public bool IsBrandNotPermitted { get; set; }
        public List<PatientMedicationsViewModel> PatientMedicines { get; set; }
    }


    public class PatientMedicationsViewModel
    {
        [Required]
        public string MedicineName { get; set; }
        [Required]
        public string DosageDirections { get; set; }
        [Required]
        public string Quantity { get; set; }
        [Required]
        public string NumberOfRepeats { get; set; }
        [Required]
        public string Date { get; set; }

    }
}
