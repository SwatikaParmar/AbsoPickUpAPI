using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class BookRehabilationService
    {
        public int BookRehabilationServiceId { get; set; }
        public int? RehabilationServicePlanId { get; set; }
        public string PatientId { get; set; }
        public string BookingDate { get; set; }
        public string BookingTime { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int BookingStatus { get; set; }
        public string AdminResponse { get; set; }
        public string ServiceType { get; set; }
        public string TransactionId { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
    }
}
