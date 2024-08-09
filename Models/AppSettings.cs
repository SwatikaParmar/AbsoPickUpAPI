using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class AppSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public int Id { get; set; }
         public string  AboutUs { get; set; }
         public string PrivacyPolicy { get; set; }
         public string TermsConditions { get; set; }
         public bool IsDeleted { get; set; }
    }
}
