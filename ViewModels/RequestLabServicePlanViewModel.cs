using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.ViewModels
{
    public partial class RequestLabServicesPlanViewModel
    {

        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public double? PlanAmount { get; set; }
        public DateTime? PlanAddedDate { get; set; }

    }
}
