using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class Appointments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required] 
        [ForeignKey("DoctorTimeSlots")]
        public long TimeSlotId { get; set; }

        [Required]
        [MaxLength(256)]
        public string DoctorId { get; set; }

        [Required]
        [MaxLength(256)]
        public string PatientId { get; set; }
        public int Status { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(256)]
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DoctorTimeSlots DoctorTimeSlots { get; set; }
    }
}
