using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientFamilyHistoryViewModel
    {
        [Required]
        public string DiseaseName { get; set; }
        [Required]
        public string MemberName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Relation { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
