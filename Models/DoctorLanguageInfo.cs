using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class DoctorLanguageInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }

        [ForeignKey("Language")]
        [Required]
        public int LanguageId { get; set; }
        public LanguageMaster Language { get; set; }
    }
}
