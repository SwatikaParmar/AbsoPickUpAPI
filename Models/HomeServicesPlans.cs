using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class HomeServicesPlans
    {
        public HomeServicesPlans()
        {
            BookHomeService = new HashSet<BookHomeService>();
        }

        public int HomeServicesPlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public double? PlanAmount { get; set; }
        public DateTime? PlanAddedDate { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual ICollection<BookHomeService> BookHomeService { get; set; }
    }
}
