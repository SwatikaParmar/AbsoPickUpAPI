using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AdminDoctorsListViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }  
        public int GenderId { get; set; }
        public string Gender { get; set; }
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPhoneNoVerified { get; set; }
        public string RegNo { get; set; }
        public int ApplicationStatus { get; set; }
        public string ApplicationStatusDetails { get; set; } 
    }
}
