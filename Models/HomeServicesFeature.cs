using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class HomeServicesFeature
    {
        public int HomeServicesFeatureId { get; set; }
        public int HomeServicesId { get; set; }
        public string FeatureDescription { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual HomeServices HomeServices { get; set; }
    }
}
