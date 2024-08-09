using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AppSettingsViewModel
    {
        public int Id { get; set; }
        [Required]
        public string AboutUs { get; set; }
        [Required]
        public string PrivacyPolicy { get; set; }
        [Required]
        public string TermsConditions { get; set; }
    }
}
