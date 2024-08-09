using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class PatientLabRecommendations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public string Id { get; set; }

        [MaxLength(450)]
        [Required]
        public string PrescriptionId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
