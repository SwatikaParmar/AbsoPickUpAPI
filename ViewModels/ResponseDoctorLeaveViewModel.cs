using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ReponseDoctorLeaveViewModel
    {
        public int DoctorRequestLeaveId { get; set; }
        public string LeaveReason { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string Status { get; set; }


    }

    
    public class ReponseDoctorLeaveListViewModel
    {
        public int DoctorRequestLeaveId { get; set; }
        public string LeaveReason { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string AdminResponse { get; set; }
        public bool? IsCancelledByUser { get; set; }
        
    }
}
