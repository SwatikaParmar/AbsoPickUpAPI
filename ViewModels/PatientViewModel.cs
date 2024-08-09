using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AddPatientViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string DialCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string DeviceType { get; set; }
        [Required]
        public string DeviceToken { get; set; }
    }

    public class PatientResponseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public string AccessToken {get; set;}
        // public int? EmailOtp {get; set;}
        // public int? PhoneOtp {get; set;}
    }
}
