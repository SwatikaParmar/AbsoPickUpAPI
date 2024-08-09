using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class HomeServices
    {
        public HomeServices()
        {
            BookHomeService = new HashSet<BookHomeService>();
            HomeServicesFeature = new HashSet<HomeServicesFeature>();
        }

        public int HomeServicesId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string ContactName { get; set; }
        public string ContactImagePath { get; set; }
        public string ContactDescription { get; set; }
        public DateTime? HomeServiceAddedDate { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }

        public virtual ICollection<BookHomeService> BookHomeService { get; set; }
        public virtual ICollection<HomeServicesFeature> HomeServicesFeature { get; set; }
    }
}
