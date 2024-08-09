using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class RequestReport
    {
        public RequestReport()
        {
            ReportDetail = new HashSet<ReportDetail>();
        }

        public int RequestReportId { get; set; }
        public string PatientEmail { get; set; }
        public string PatientId { get; set; }
        public int? Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<ReportDetail> ReportDetail { get; set; }
    }
}
