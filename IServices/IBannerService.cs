using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IBannerService
    {
        bool CreateBanner(Banner _banner);
        List<BannerResponseViewModel> GetBannerList();
    }
}
