using AnfasAPI.Common;
using AnfasAPI.Data; 
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class BannerService : IBannerService
    {

        private ApplicationDbContext _context;

        public BannerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateBanner(Banner _banner)
        {
            try
            {
                _context.Banner.Add(_banner);
                _context.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public List<BannerResponseViewModel> GetBannerList()
        {
            try
            {
                if (_context != null)
                { 
                    var source = (from b in _context.Banner 
                                  orderby b.CreateDate descending
                                  select new BannerResponseViewModel
                                  {
                                      BannerId = b.BannerId, 
                                      BannerImage = b.BannerImage,
                                      BannerType = b.BannerType,
                                      BannerTypeId = b.BannerTypeId,
                                  }).AsQueryable();

                    var src = source.ToList();
                    return src;
                }
                return null;
            }
            catch(Exception)
            {
                throw;
            }  
       }


    }

}

