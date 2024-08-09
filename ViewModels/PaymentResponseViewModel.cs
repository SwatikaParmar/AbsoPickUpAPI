using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PaymentResponseViewModel
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionType { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public string CustomerId { get; set; }
        public string FailureCode{ get; set; }
        public string FailureMessage { get; set; }
        public DateTime PaymentDate { get; set; } 
    }
}
