using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class DoctorRequestLeave
    {
        public int DoctorRequestLeaveId { get; set; }
        public string DoctorId { get; set; }
        public string LeaveReason { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string AdminResponse { get; set; }
        public bool? IsCancelledByUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
