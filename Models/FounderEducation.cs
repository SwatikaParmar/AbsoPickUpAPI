using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class FounderEducation
    {
        public int FounderEducationId { get; set; }
        public int FounderInfoId { get; set; }
        public string EducationType { get; set; }
        public string EducationFrom { get; set; }
    }
}
