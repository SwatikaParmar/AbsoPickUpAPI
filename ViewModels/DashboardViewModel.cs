using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DashboardViewModel
    {
        public int totalDoctors { get; set; }
        public int totalAppointments { get; set; }
        public int totalScheduledAppointments { get; set; }
        public List<BlogListViewModel> blogList {get; set;}
        public List<AppointmentListViewModel> appointmentList {get; set;}

    }
}