using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetPatientInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string DateOfBirth { get; set; }
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPhoneNoVerified { get; set; }
        public string ProfilePic { get; set; }
        public string City { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public int StateId { get; set; }
        public string State { get; set; }
        public string CurrentAddress { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string BloodGroup { get; set; }
        public bool IsVegetarian { get; set; }
        public bool UseAlcohol { get; set; }
        public bool UseSmoke { get; set; }
        public bool UseDrug { get; set; }

    }
}
