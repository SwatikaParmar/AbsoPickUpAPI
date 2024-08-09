using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class PatientPastMedicalHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(256)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string TreatmentName { get; set; }
        [MaxLength(50)]
        public string DoctorName { get; set; }
        public DateTime? Date { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
    }
}
