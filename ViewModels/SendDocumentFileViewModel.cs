using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class SendDocumentFileViewModel
    {
        public string ToId { get; set; }
        public string AppointmentId { get; set; }
        public IFormFile DocumentFile { get; set; }
        public string Type { get; set; }
    }
}
