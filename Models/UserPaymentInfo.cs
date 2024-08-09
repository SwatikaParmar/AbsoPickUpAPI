using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class UserPaymentInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string PatientId { get; set; }
        [Required]
        [MaxLength(256)]
        public string DoctorId { get; set; }
        [Required]
        [MaxLength(450)]
        public string AppointmentId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        [Required]
        public string PaymentMode { get; set; }
        [Required]
        public int TransactionType { get; set; }
        [Required]
        public int PaymentStatus { get; set; }
        [Required]
        public string TransactionId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
