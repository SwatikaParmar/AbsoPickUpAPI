using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class DoctorSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan fromTime { get; set; }
        [Required]
        public TimeSpan toTime { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(256)]
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }

}
