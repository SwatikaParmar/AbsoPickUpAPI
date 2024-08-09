using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class DoctorEducationInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }

        [ForeignKey("Degree")]
        [Required]
        public int DegreeId { get; set; }

        public string Degree { get; set; }
        [MaxLength(200)]
        public string InstituteName { get; set; }
       

    }

}
