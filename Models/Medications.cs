using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class Medications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string PrescriptionId { get; set; }

        [MaxLength(200)]
        [Required]
        public string MedicineName { get; set; }

        [Required]
        public string DosageDirections { get; set; }
        [Required]
        public string Quantity { get; set; }

        [Required]
        public string NumberOfRepeats { get; set; } 
        public DateTime Date { get; set; }

    }
}
