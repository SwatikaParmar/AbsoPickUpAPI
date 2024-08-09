using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class LabServicesPlan
    {
        public LabServicesPlan()
        {
            BookLabService = new HashSet<BookLabService>();
        }

        public int LabServicesPlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public double? PlanAmount { get; set; }
        public DateTime? PlanAddedDate { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual ICollection<BookLabService> BookLabService { get; set; }
    }
}
