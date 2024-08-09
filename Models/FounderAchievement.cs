using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.Models
{
    public partial class FounderAchievement
    {
        public int FounderAchievementId { get; set; }
        public int FounderInfoId { get; set; }
        public string AchievementDescription { get; set; }
    }
}
