using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class DialysisServicesFeature
    {
        public int DialysisServicesFeatureId { get; set; }
        public int DialysisServicesId { get; set; }
        public string FeatureDescription { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual DialysisServices DialysisServices { get; set; }
    }
}
