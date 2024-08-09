using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class FounderPersonalAchievement
    {
        public int FounderPersonalAchievementId { get; set; }
        public int FounderInfoId { get; set; }
        public string Description { get; set; }
    }
}
