using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetAllergyHistoryViewModel
    {
        public long Id { get; set; }
        public string AllergyName { get; set; }
        public string Description { get; set; }
    }
}
