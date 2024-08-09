using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseRehabilationServicesViewModel
    {
        public int RehabilationServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? RehabilationServiceAddedDate { get; set; }
        public List<ResponseRehabilationServicesFeatureViewModel> RehabilationServiceFeatures {get; set;}

    }

    public class ResponseRehabilationServicesFeatureViewModel
    {
        public string FeatureDescription { get; set; }
    }

}
