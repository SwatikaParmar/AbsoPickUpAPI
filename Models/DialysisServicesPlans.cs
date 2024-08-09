using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class DialysisServicesPlans
    {
        public int DialysisServicesPlanId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public double? Value { get; set; }
        public string SpecialtyName { get; set; }
    }
}
