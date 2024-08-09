using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class LabServicesFeature
    {
        public int LabServicesFeatureId { get; set; }
        public int? LabServicesId { get; set; }
        public string FeatureDescription { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual LabServices LabServices { get; set; }
    }
}
