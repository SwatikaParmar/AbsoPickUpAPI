using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UpdatePatientBasicInfoVM
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int GenderId { get; set; } 
        public string PhoneNumber { get; set; }
        [Required]
        public string DialCode { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string CurrentAddress { get; set; }
    }
}
