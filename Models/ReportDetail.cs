using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class ReportDetail
    {
        public int ReportDetailId { get; set; }
        public int? RequestReportId { get; set; }
        public int? BookServiceId { get; set; }
        public string ReportName { get; set; }
        public string ReportDescription { get; set; }
        public string PatientId { get; set; }
        public int ServiceId { get; set; }
        public string ReportPath { get; set; }
        public int? Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual RequestReport RequestReport { get; set; }
    }
}
