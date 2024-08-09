using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class BookHomeService
    {
        public int BookHomeServiceId { get; set; }
        public int HomeServiceId { get; set; }
        public int HomeServicesPlanId { get; set; }
        public string PatientId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Reason { get; set; }
        public string AddressStreet { get; set; }
        public string AddressLat { get; set; }
        public string AddressLong { get; set; }
        public string AddressCountry { get; set; }
        public int Status { get; set; }
        public string AdminResponse { get; set; }
        public string ServiceType { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public bool? IsAssignedDoctor { get; set; }
        public string DoctorId { get; set; }

        public virtual HomeServices HomeService { get; set; }
        public virtual HomeServicesPlans HomeServicesPlan { get; set; }
    }
}
