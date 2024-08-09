using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IPaymentService
    {
        PaymentResponseViewModel ProcessPayment(PaymentViewModel model, string userId);
        bool ProcessPayout(string userId);
    }
}
