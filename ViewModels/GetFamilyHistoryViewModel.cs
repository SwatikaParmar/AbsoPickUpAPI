using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetFamilyHistoryViewModel
    {
        public long Id { get; set; }
        public string DiseaseName { get; set; }
        public string MemberName { get; set; }
        public int Age { get; set; }
        public string Relation { get; set; }
        public string Description { get; set; }
    }
}
