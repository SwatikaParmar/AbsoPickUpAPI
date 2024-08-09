using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.ViewModels
{
    public partial class RequestUpdateRadiologyServicesPlanViewModel
    {

        public int RadiologyServicesPlanId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public float Value { get; set; }
        public string SpecialtyName { get; set; }

    }
}
