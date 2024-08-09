using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class RadiologyServicesFeature
    {
        public int RadiologyServicesFeatureId { get; set; }
        public int RadiologyServicesId { get; set; }
        public string FeatureDescription { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual RadiologyServices RadiologyServices { get; set; }
    }
}
