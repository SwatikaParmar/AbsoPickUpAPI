using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class FounderInfo
    {
        public int FounderInfoId { get; set; }
        public string FounderName { get; set; }
        public string FounderDesignation { get; set; }
        public string FounderImage { get; set; }
        public DateTime? Createdate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
