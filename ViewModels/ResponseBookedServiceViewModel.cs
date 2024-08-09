using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnfasAPI.ViewModels
{
    public class ResponseBookedServiceViewModel
    {
        public string patientId { get; set; }
        public int bookingId { get; set; }
        public int serviceId { get; set; }
        public int? requestReportId { get; set; }
        public int? servicePlanId { get; set; }
        public int reportDetailId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int patientAge { get; set; }     
        public string bookingDate { get; set; }
        public string BookingTime { get; set; }
        public string reason { get; set; }
        public string serviceType { get; set; }
        public string status { get; set; }
        public string reportLink { get; set; }
    }
    public class UploadBookedServiceReportViewModel
    {
        public int bookingId { get; set; }
        public int serviceId { get; set; }
        public int requestReportId { get; set; }
        public string reportName { get; set; }
        public string reportDescription { get; set; }

    }
    public class ResendReportViewModel
    {
        public int reportDetailId { get; set; }
    }
}
