using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AddBannerViewModel
    {
        public string BannerType { get; set; }
        public string BannerTypeId { get; set; }
        public IFormFile BannerImage { get; set; }
    }
}
