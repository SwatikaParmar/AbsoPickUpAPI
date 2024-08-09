using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseHomeServicesViewModel
    {
        public int HomeServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? HomeServiceAddedDate { get; set; }
        public List<ResponseHomeServicesFeatureViewModel> HomeServiceFeatures {get; set;}

    }

    public class ResponseHomeServicesFeatureViewModel
    {
        public int HomeServicesFeatureId { get; set; }
        public string FeatureDescription { get; set; }
    }

}
