using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class ResponseServicesPlanViewModel
    {

        public int ServicesPlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public double? PlanAmount { get; set; }
        public DateTime? PlanAddedDate { get; set; }

    }
}
