using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class PatientHealthInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(256)]
        public string UserId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Height { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }
        [MaxLength(3)]
        public string BloodGroup { get; set; }
        public bool IsVegetarian { get; set; }
        public bool UseAlcohol { get; set; }
        public bool UseSmoke { get; set; }
        public bool UseDrug { get; set; }
    }
}
