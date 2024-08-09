﻿using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class DialysisServices
    {
        public DialysisServices()
        {
            DialysisServicesFeature = new HashSet<DialysisServicesFeature>();
        }

        public int DialysisServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? DialysisServiceAddedDate { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual ICollection<DialysisServicesFeature> DialysisServicesFeature { get; set; }
    }
}
