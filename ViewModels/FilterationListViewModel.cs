using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class FilterationListViewModel
    {
        public string sortOrder { get; set; }
        public string sortField { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string searchQuery { get; set; }
        public string filterBy { get; set; } = "";
    }
}
