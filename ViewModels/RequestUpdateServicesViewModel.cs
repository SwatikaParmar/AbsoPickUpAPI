using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestUpdateServicesViewModel
    {
        public int ServicesId { get; set; }
        public string Description { get; set; }
        public string ContactName { get; set; }
        public string ContactDescription { get; set; }
        public int ServiceTypeValue { get; set; }
        public List<RequestUpdateServicesFeatureViewModel> ServiceFeatures {get; set;}

    }

    public class RequestUpdateServicesFeatureViewModel
    {   
        public int ServicesFeatureId { get; set; }
        public string FeatureDescription { get; set; }
    }

}
