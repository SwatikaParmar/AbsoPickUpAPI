using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestRadiologyRadiologyServiceModel
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public double? PlanAmount { get; set; }
        public DateTime? PlanAddedDate { get; set; }
    }

}
