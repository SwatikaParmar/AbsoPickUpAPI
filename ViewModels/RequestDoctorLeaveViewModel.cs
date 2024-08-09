using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestDoctorLeaveViewModel
    {
        public string LeaveReason { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
