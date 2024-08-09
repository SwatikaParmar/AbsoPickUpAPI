using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class DoctorBankInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }

        [MaxLength(256)]
        public string BankName { get; set; }
        [MaxLength(100)]
        public long AccountNumber { get; set; }
        [MaxLength(100)]
        public string RouteNo { get; set; }
        [MaxLength(50)]
        public string BranchCode { get; set; }
        [MaxLength(50)]
        public string PostCode { get; set; }
        [MaxLength(256)]
        public string Address { get; set; }

    }
}
