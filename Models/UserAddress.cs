using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class UserAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } 
        public int NationalityId { get; set; }
        [Required] 
        public int CountryId { get; set; }
        [Required] 
        public int StateId { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(100)]
        public string Landmark { get; set; }
        [MaxLength(200)]
        public string CurrentAddress { get; set; }
        [MaxLength(200)]
        public string PermanentAddress { get; set; }
        [MaxLength(20)]
        public string ZipCode { get; set; } 
    }
}
