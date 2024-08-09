using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseBookHomeServiceViewModel
    {
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
    }


}
