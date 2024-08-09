using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestUpdateLabServicesViewModel
    {
        public int LabServicesId { get; set; }
        public string Description { get; set; }
        public string ContactName { get; set; }
        public string ContactDescription { get; set; }
        public List<RequestUpdateLabServicesFeatureViewModel> LabServiceFeatures {get; set;}

    }

    public class RequestUpdateLabServicesFeatureViewModel
    {   
        public int LabServicesFeatureId { get; set; }
        public string FeatureDescription { get; set; }
    }

}
