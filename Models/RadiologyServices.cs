using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class RadiologyServices
    {
        public RadiologyServices()
        {
            //BookRadiologyService = new HashSet<BookRadiologyService>();
            RadiologyServicesFeature = new HashSet<RadiologyServicesFeature>();
        }

        public int RadiologyServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? RadiologyServiceAddedDate { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        //public virtual ICollection<BookRadiologyService> BookRadiologyService { get; set; }
        public virtual ICollection<RadiologyServicesFeature> RadiologyServicesFeature { get; set; }
    }
}
