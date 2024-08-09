using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class DoctorTimeSlots
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan SlotFrom { get; set; }
        [Required]
        public TimeSpan SlotTo { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedTime { get; set; }
        public bool IsSlotAvailable { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(256)]
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
