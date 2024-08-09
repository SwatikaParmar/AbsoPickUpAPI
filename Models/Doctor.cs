using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class Doctor
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string Discriminator { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Otp { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public string DialCode { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public string RegNo { get; set; }
        public string About { get; set; }
        public decimal? Experience { get; set; }
        public string ProfilePic { get; set; }
        public string GoogleId { get; set; }
        public string FacebookId { get; set; }
        public bool? IsSocialUser { get; set; }
        public int? LastScreenId { get; set; }
        public int? ApplicationStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string MedicalRegNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
    }
}
