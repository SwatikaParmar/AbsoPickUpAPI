using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class PrivacyPolicy
    {
        public int PrivacyPolicyId { get; set; }
        public string PrivacyPolicyContent { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
