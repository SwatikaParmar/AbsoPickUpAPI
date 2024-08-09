using AnfasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IWalletService
    {
        bool AddUpdateAppointmentFee(WalletBillingInfo model);
        bool UpdateWallet(WalletBillingInfo model);
        Task<WalletBillingInfo> GetUserWalletInfo(string userId);
    }
}
