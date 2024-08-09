using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestBookLabServiceViewModel
    {
        public int? LabServiceId { get; set; }
        public int? LabServicesPlanId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string BookingTime { get; set; }
        public string Reason { get; set; }
    }
}
