using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class TransactionMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string AppointmentId { get; set; }
        [Required]
        [MaxLength(450)]
        public string PatientId { get; set; }
         
        [MaxLength(256)]
        public string OrderId { get; set; }
         
        [MaxLength(256)]
        public string ReferenceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentStatus { get; set; }

        [Required]
        [MaxLength(50)]
        public string TransactionType { get; set; }

        [Required]
        [MaxLength(200)]
        public string PaymentMode { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } 
    }
}
