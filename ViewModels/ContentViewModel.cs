using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ContentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ContentDomainViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class SpecialityContentDomainViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsActive { get; set; }
    }
}
