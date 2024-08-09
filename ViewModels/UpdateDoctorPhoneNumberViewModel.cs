using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UpdateDoctorPhoneNumberViewModel
    {
        [Required]
        public string DialCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
