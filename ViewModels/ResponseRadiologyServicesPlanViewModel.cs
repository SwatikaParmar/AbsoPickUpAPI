using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseRadiologyServicesPlanViewModel
    {
        public int RadiologyServicesPlanId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public double? Value { get; set; }
        public string SpecialtyName { get; set; }

    }

}
