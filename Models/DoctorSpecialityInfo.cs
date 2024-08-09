using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{

    
    public class DoctorSpecialityInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }

        [ForeignKey("Speciality")]
        [Required]
        public int SpecialityId { get; set; }  
        public SpecialityMaster Speciality { get; set; }
    } 
}
