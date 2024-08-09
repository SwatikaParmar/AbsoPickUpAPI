using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetUserReviewViewModel
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public string Reviews { get; set; }
        public string Date { get; set; }
    }
}
