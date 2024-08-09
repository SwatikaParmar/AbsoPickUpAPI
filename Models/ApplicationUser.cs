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
    public class ApplicationUser : IdentityUser
    {
        [Required] 
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required] 
        [MaxLength(50)]
        public string LastName { get; set; }
        public int? Otp { get; set; }  
        public DateTime? DateOfBirth { get; set; }         
        [Required]
        public int GenderId { get; set; }
        [Required]
        [MaxLength(10)]
        public string DialCode { get; set; }
        [Required]
        public string DeviceToken { get; set; }
        [Required]
        public string DeviceType { get; set; }
        [MaxLength(20)]
        public string RegNo { get; set; }
        public string About { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Experience { get; set; }
       
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Education { get; set; }
        [MaxLength(500)]
        public string ProfilePic { get; set; }
        [MaxLength(256)]
        public string GoogleId { get; set; }
        [MaxLength(256)]
        public string FacebookId { get; set; }
        public bool IsSocialUser { get; set; }
        public int LastScreenId { get; set; }
        public int ApplicationStatus { get; set; }
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [MaxLength(256)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } 
        public bool IsDeleted { get; set; }
        [MaxLength(256)]
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string MedicalRegistrationNumber { get; set; }
        
    }
}
