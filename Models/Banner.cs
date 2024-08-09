using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class Banner
    {
        public int BannerId { get; set; }
        public string BannerType { get; set; }
        public string BannerTypeId { get; set; }
        public string BannerImage { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
