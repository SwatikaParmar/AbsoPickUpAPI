using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestUpdateRadiologyServicesViewModel
    {
        public int RadiologyServicesId { get; set; }
        public string Description { get; set; }
        public string ContactName { get; set; }
        public string ContactDescription { get; set; }
        public List<RequestUpdateRadiologyLabServicesFeatureViewModel> RadiologyServiceFeatures {get; set;}

    }

    public class RequestUpdateRadiologyLabServicesFeatureViewModel
    {   
        public int RadiologyServicesFeatureId { get; set; }
        public string FeatureDescription { get; set; }
    }

}
