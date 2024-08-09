using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public string UserId { get; set; }
        public double? PaymentAmount { get; set; }
        public int? ServiceStatus { get; set; }
        public int? ServiceType { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
