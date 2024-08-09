using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace AnfasAPI.Models
{
    public class UploadServiceImage
    {
    
        public IFormFile ServiceImage { get; set; }
        public int ServiceId { get; set; }
        public int ServiceTypeValue { get; set; }
    }
}
