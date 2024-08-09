using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace AnfasAPI.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string DeviceType { get; set; }
        [Required]
        public string DeviceToken { get; set; } 
    }

    public class SocialLoginViewModel
    {
        [Required]
        public string LoginType { get; set; }
        
        [MaxLength(50)]
        public string FirstName { get; set; }
        
        [MaxLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [Required]
        public string SocialId { get; set; }

        [Required]
        public string DeviceType { get; set; }

        [Required]
        public string DeviceToken { get; set; }
       
    }

    public class DoctorSocialLoginViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string LoginType { get; set; }
        [Required]
        public string SocialId { get; set; }
        [Required]
        public int GenderId { get; set; }
        public string DateOfBirth { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string DeviceType { get; set; }
        [Required]
        public string DeviceToken { get; set; }
        [Required]
        public string DialCode { get; set; }
    }
    public class EmailModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool? IsUserExisting {get; set;}
    }
    public class ResetUserModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string otp { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
    public class VerifyUserModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string otp { get; set; }
    }
    public class VerifyPhoneModel
    {
        [Required]
        public string code { get; set; }
    }
    public class PhoneModel
    {
        [Required]
        public string DialCode { get; set; }
        [Required] 
        public string PhoneNo { get; set; }
    }
}
