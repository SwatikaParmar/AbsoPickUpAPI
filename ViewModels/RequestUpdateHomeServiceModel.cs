using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class RequestupdateServicePlanViewModel
    {
        public int ServicesPlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public double? PlanAmount { get; set; }
        public DateTime? PlanAddedDate { get; set; }
        public int ServiceTypeValue { get; set; }
    }

}
