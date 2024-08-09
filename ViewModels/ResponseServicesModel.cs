using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseServicesViewModel
    {
        public int ServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? ServiceAddedDate { get; set; }

        public List<ResponseServicesFeatureViewModel> ServiceFeatures {get; set;}

    }

    public class ResponseServicesFeatureViewModel
    {
        public string FeatureDescription { get; set; }
        public int ServicesFeatureId { get; set; }
    }

}
