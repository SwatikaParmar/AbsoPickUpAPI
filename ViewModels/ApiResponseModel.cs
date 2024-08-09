using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ApiResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public int code { get; set; }
    }
}
