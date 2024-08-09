using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class RehabilationServicesFeature
    {
        public int RehabilationServicesFeatureId { get; set; }
        public int RehabilationServicesId { get; set; }
        public string FeatureDescription { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual RehabilationServices RehabilationServices { get; set; }
    }
}
