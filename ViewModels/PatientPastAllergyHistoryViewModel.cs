using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientPastAllergyHistoryViewModel
    {
        [Required]
        public string AllergyName { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
