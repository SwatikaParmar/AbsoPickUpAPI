using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetMedicalReportViewModel
    {
        public long Id { get; set; }
        public string ReportName { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string MedicalDocumentPath { get; set; }
    }
}
