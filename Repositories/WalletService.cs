using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class WalletService : IWalletService
    {
        private ApplicationDbContext _context;
        public WalletService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddUpdateAppointmentFee(WalletBillingInfo model)
        {
            try
            {
                bool ifExists = _context.WalletBillingInfo.Any(x => x.UserId == model.UserId);
                if (ifExists)
                {
                    var data = _context.WalletBillingInfo.Where(x => x.UserId == model.UserId).FirstOrDefault();
                    data.Fee = model.Fee;
                    data.UpdatedDate = DateTime.UtcNow;
                    _context.WalletBillingInfo.Update(data);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    _context.WalletBillingInfo.Add(model);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateWallet(WalletBillingInfo model)
        {
            try
            {
                var data = _context.WalletBillingInfo.Where(x => x.UserId == model.UserId).FirstOrDefault();
                data.Balance = model.Balance;
                data.UpdatedDate = DateTime.UtcNow;
                _context.WalletBillingInfo.Update(data);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<WalletBillingInfo> GetUserWalletInfo(string userId)
        {
            try
            {
                return await _context.WalletBillingInfo.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
