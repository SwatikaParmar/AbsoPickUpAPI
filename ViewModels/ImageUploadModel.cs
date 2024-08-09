using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnfasAPI.Models
{
    public class ImageUploadModel
    {
        public int FounderInfoId { get; set; }
        [Required]
        public IFormFile ImgFile { get; set; }
    }
}
