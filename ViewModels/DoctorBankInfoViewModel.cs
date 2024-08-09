using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorBankInfoViewModel
    {
        [Required]
        public string BankName { get; set; }
        [Required]

        public long AccountNumber { get; set; }
        [Required]

        public string RouteNo { get; set; }
        [Required]

        public string BranchCode { get; set; }
        [Required]

        public string PostCode { get; set; }
        [Required]

        public string Address { get; set; }
    }
}
