using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class BannerResponseViewModel
    {
        public int BannerId { get; set; }
        public string BannerType { get; set; }
        public string BannerTypeId { get; set; }
        public string BannerImage { get; set; }
    }
}
