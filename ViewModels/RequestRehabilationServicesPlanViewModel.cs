using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestRehabilationServicesPlanViewModel
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public float Value { get; set; }
        public string SpecialtyName { get; set; }
    }

}
