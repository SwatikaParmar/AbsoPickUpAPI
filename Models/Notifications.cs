using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [MaxLength(450)]
        [Required]
        public string FromUser { get; set; }

        [MaxLength(450)]
        [Required]
        public string ToUser { get; set; } 
        
        [Required]
        public int Type { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Purpose { get; set; }

        [MaxLength(450)]
        [Required]
        public string PurposeId { get; set; } 
        public bool IsRead { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
