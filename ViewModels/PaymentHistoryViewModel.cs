using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PaymentHistoryViewModel
    {

        public double? PaymentAmount { get; set; }
        public string ServiceStatus { get; set; }
        public string ServiceType { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
