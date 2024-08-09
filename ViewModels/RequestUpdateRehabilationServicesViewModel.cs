using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestUpdateRehabilationServicesViewModel
    {
        public int RehabilationServicesId { get; set; }
        public string Description { get; set; }
        public string ContactName { get; set; }
        public string ContactDescription { get; set; }
        public List<RequestUpdateRehabilationLabServicesFeatureViewModel> RadiologyServiceFeatures {get; set;}

    }

    public class RequestUpdateRehabilationLabServicesFeatureViewModel
    {   
        public int RadiologyServicesFeatureId { get; set; }
        public string FeatureDescription { get; set; }
    }

}
