using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetDoctorDetailsViewModel
    {
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public decimal Rating { get; set; }
        public int ReviewsCount { get; set; }
        public string About { get; set; }
        public decimal AppointmentFees { get; set; }
        public List<LanguageInfoViewModel> DoctorLangauges { get; set; }
        public List<SpecialityInfoViewModel> DoctorSpecialities { get; set; }
        public List<EducationInfoViewModel> DoctorEducation { get; set; }
    }
}
