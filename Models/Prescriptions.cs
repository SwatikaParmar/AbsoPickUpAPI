using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class Prescriptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public string Id { get; set; }
        [MaxLength(450)]
        [Required]
        public string PatientId { get; set; }
        [MaxLength(450)]
        [Required]
        public string DoctorId { get; set; }
        [MaxLength(450)]
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
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
