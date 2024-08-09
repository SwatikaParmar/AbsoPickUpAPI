using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class UserRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string AppointmentId { get; set; }
        [Required]
        [MaxLength(256)]
        public string DoctorId { get; set; }
        [Required]
        [MaxLength(256)]
        public string PatientId { get; set; }
        public int Rating { get; set; } 
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
