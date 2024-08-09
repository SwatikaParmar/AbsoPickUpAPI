using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetTopDoctorsViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Rating { get; set; }
        public decimal AppointmentFees { get; set; }
        public string ProfilePic { get; set; }
        public List<SpecialityInfoViewModel> DoctorSpecialities { get; set; }
    }
    
}
