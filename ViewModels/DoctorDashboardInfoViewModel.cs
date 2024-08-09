using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorDashboardInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public List<SpecialityInfoViewModel> DoctorSpecialities { get; set; }
        public decimal TotalEarnings { get; set; }
        public int PercentageProfileComplete { get; set; }
        public int TotalUpcomingAppointments { get; set; }
        public int TotalPendingAppointments { get; set; }
        public int TotalPastAppointments { get; set; }
    }
}
