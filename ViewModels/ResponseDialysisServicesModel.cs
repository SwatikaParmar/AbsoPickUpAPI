using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseDialysisServicesViewModel
    {
        public int DialysisServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? DialysisServiceAddedDate { get; set; }
        public List<ResponseDialysisServicesFeatureViewModel> DialysisServiceFeatures {get; set;}

    }

    public class ResponseDialysisServicesFeatureViewModel
    {
        public string FeatureDescription { get; set; }
    }

}
