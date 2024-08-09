using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestBookDialysisServiceViewModel
    {
        public int DialysisServicePlanId { get; set; }
        public string BookingDate { get; set; }
        public string BookingTime { get; set; }
        public string Reason { get; set; }
    }
}
