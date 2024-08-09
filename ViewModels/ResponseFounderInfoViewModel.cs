using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnfasAPI.Models;

namespace AnfasAPI.ViewModels
{
    public class ResponseFounderInfoViewModel
    {
        public int FounderInfoId{ get; set; }
        public string FounderName { get; set; }
        public string FounderDesignation { get; set; }
        public string FounderImage { get; set; }
        public List<ResponseFounderEducationViewModel> FounderEducationList { get; set; }
        public List<ResponseFounderAdditionalInformationViewModel> FounderAdditionalInformationList { get; set; }
        public List<ResponseFounderAchievementViewModel> FounderAchievementList { get; set; }
        public List<ResponseFounderPersonalAchievementViewModel> FounderPersonalAchievementList { get; set; }

    }

}
