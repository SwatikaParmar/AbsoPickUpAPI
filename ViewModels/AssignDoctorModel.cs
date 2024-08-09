using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnfasAPI.ViewModels
{
    public class AssignDoctorViewModel
    {
        public string DoctorId { get; set; }
        public int BookingId { get; set; }
        public int ServiceTypeValue { get; set; }
    }
}