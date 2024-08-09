using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PaymentViewModel
    {
        public string StripeToken { get; set; }
        public long? Amount { get; set; }
        public string Email { get; set; }
        public string AppointmentId { get; set; }
    }
}
