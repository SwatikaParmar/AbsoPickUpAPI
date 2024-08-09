using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorWorkInfoViewModel
    {
        public decimal? Experience { get; set; }
        public string RegNo { get; set; }
        public decimal AppointmentFees { get; set; }
        public List<SpecialityInfoViewModel> DoctorSpecialities { get; set; }
        public List<EducationInfoViewModel> DoctorEducation { get; set; }
    }
    public class SpecialityInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class EducationInfoViewModel
    {
        public int Id { get; set; }
        public string InstituteName { get; set; }
        public string DegreeName { get; set; }
    }
}
