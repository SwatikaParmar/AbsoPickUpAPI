using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UpdateBlogStatusViewModel
    {
        public string Id { get; set; }
        public bool IsAdminApproved { get; set; }
    }
}
