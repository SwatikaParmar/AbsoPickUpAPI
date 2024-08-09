using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace AnfasAPI.Models
{
    public class UploadDoctorImage
    {
        public IFormFile DoctorImage { get; set; }
        public string Id { get; set; }
    }
    public class UploadPatientReport
    {
        [Required]
        public int bookingId { get; set; }
        [Required]
        public int serviceId { get; set; }
        [Required]
        public int requestReportId { get; set; }
        [Required]
        public string reportName { get; set; }
        public string reportDescription { get; set; }
        [Required]
        public IFormFile reportFile { get; set; }
    }
}
