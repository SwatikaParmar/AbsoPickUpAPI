using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ResponseBlogDetailViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BlogImagePath { get; set; }
        public string DoctorName { get; set; }
        public string DoctorImagePath { get; set; }
        public string CreatedDate { get; set; }
        public bool IsAdminApproved { get; set; }
        public int? BlogStatus {get; set;}
        public string BlogStatusDisplay {get; set;}
    }
}
