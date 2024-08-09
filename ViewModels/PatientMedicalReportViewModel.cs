using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientMedicalReportViewModel
    {
        [Required] 
        public string ReportName { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public IFormFile MedicalDocument { get; set; }
    }
}
