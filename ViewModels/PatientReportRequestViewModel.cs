using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnfasAPI.ViewModels
{
    public class PatientReportRequestViewModel
    {
        public int requestReportId { get; set; }
        public string patientId { get; set; }
        public string firstName { get; set; }
        public string LastName { get; set; }
        public int  patientAge { get; set; }
        public string patientEmail { get; set; }      
        public String  status { get; set; }
        public String  requestDate { get; set; }
        public int  totalTestCount { get; set; }
    }
}