using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class SendImageFileViewModel
    {
        public string ToId { get; set; }
        public string AppointmentId { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Type { get; set; }
    }
}
