using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnfasAPI.ViewModels
{
    public class UpdateDoctorDetailsViewModels
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        //public string  ProfilePic { get; set; }
        public decimal? Experience { get; set; }
        public string DegreeId { get; set; }
        public string DialCode { get; set; }
        public string MedicalRegistrationNumber { get; set; }
        public int CountryId {get; set;}
        public int StateId {get; set;}
        public string City {get; set;}
        public string Address {get; set;}
        //public string DegreeId {get; set;}
        public string DoctorLanguage {get;set;}
        public string DoctorSpecialization {get;set;}
        public string About { get; set; }
 
    }

}
