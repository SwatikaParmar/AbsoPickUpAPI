using System;
using System.Collections.Generic;

#nullable disable

namespace AnfasAPI.ViewModels
{
    public partial class RequestupdateUpdateFounderInfoViewModel
    {
        public int FounderInfoId { get; set; }
        public string FounderName { get; set; }
        public string FounderDesignation { get; set; }

        //public string FounderImage { get; set; }
        //public DateTime? Createdate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public List<RequestFounderEducationViewModel> FounderEducationList { get; set; }
        public List<RequestFounderAdditionalInformationViewModel> FounderAdditionalInformationList { get; set; }
        public List<RequestUpdateFounderAchievementViewModel> FounderAchievementList { get; set; }
        public List<RequestUpdateFounderPersonalAchievementViewModel> FounderPersonalAchievementList { get; set; }
    }
}
