using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorBasicInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string About { get; set; }
        public string ProfilePic { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPhoneNoVerified { get; set; }
        public string DialCode { get; set; }
        public string CurrentAddress { get; set; }
        public int CountryId { get; set; }
        public string Country{ get; set; }
        public int StateId { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int NationalityId { get; set; }
        public string Nationality { get; set; }
        public List<LanguageInfoViewModel> DoctorLangauges { get; set; }

    }
    public class LanguageInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
