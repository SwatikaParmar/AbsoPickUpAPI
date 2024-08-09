using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class PatientFamilyHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(256)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string DiseaseName { get; set; }
        [MaxLength(50)]
        public string MemberName { get; set; }
        public int Age { get; set; }
        [MaxLength(50)]
        public string  Relation { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }


    }
}
