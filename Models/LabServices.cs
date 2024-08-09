using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class LabServices
    {
        public LabServices()
        {
            BookLabService = new HashSet<BookLabService>();
            LabServicesFeature = new HashSet<LabServicesFeature>();
        }

        public int LabServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? LabServiceAddedDate { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual ICollection<BookLabService> BookLabService { get; set; }
        public virtual ICollection<LabServicesFeature> LabServicesFeature { get; set; }
    }
}
