using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class RadiologyServicesPlans
    {
        public RadiologyServicesPlans()
        {
            BookRadiologyService = new HashSet<BookRadiologyService>();
        }

        public int RadiologyServicesPlanId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public double? Value { get; set; }
        public string SpecialtyName { get; set; }

        public virtual ICollection<BookRadiologyService> BookRadiologyService { get; set; }
    }
}
