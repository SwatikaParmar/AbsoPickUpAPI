using AnfasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorProfileInfoViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneNoVerified { get; set; }
        public int ApplicationStatus { get; set; }
        public string ApplicationStatusDetails { get; set; } 
        public string ProfilePic { get; set; } 
    }

    public class DoctorProfileDetailsViewModel
    {
        public DoctorProfileInfoViewModel DoctorProfileInfo { get; set; }
        public DoctorWorkInfoViewModel DoctorWorkInfo { get; set; }
        public DoctorBasicInfoViewModel DoctorBasicInfo { get; set; }
        public DoctorBankInfo DoctorBankInfo { get; set; }
    }
}
