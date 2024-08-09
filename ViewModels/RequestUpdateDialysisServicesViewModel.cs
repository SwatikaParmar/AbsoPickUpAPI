using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestUpdateDialysisServicesViewModel
    {
        public int DialysisServicesId { get; set; }
        public string Description { get; set; }
        public string ContactName { get; set; }
        public string ContactDescription { get; set; }
        public List<RequestUpdateDialysisLabServicesFeatureViewModel> RadiologyServiceFeatures {get; set;}

    }

    public class RequestUpdateDialysisLabServicesFeatureViewModel
    {   
        public int RadiologyServicesFeatureId { get; set; }
        public string FeatureDescription { get; set; }
    }

}
