using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientRegistrationViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        //[Required]
        //[DataType(DataType.EmailAddress)]
        //public string Email { get; set; }
        //[Required]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }
        [Required]
        public int GenderId { get; set; }
        public string DateOfBirth { get; set; }
        [Required]
        public string PhoneNumber { get; set; } 
        //[Required]
        //public string DeviceType { get; set; } 
        //[Required]
        //public string DeviceToken { get; set; } 
        [Required]
        public string DialCode { get; set; }
    }
}
