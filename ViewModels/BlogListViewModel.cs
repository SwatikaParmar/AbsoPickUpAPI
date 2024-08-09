using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class BlogListViewModel
    {
        public string blogId {get; set;}
        public string userId {get; set;}
        public string title { get; set; }
        public string description { get; set; }
        public string blogImagePath { get; set; }
        public bool isAdminApproved {get; set;}
        public int? blogStatus {get; set;}
         public string blogStatusDisplay {get; set;}

    }
}