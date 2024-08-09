using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorPersonalInfoViewModel
    {
        public int[] specialityId { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public string City { get; set; }
        public string Address { get; set; } 
        [Required]
        public string RegistrationNo { get; set; }
    }
}
