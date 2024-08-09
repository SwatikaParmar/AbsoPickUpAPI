using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class AppointmentMediaSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string AppointmentId { get; set; }

        [Required]
        [MaxLength(450)]
        public string DoctorId { get; set; }

        [Required]
        [MaxLength(450)]
        public string PatientId { get; set; }

        [Required]
        [MaxLength(450)]
        public string SessionId { get; set; } 
        public string connectionId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpireTime { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SessionDetails { get; set; }
        public int SessionStatus { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedTime { get; set; }
    }
}
