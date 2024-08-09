using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorScheduleViewModel
    {
        public string[] AvailableDates { get; set; }
        public TimeSlots[] TimeSlots { get; set; }
        public string UserId { get; set; }
        public bool IsAvailable { get; set; }
        

    }
   
    public class AvailableDoctorSchedule
    {
        public string[] AvailableDates { get; set; }
        public TimeSlots[] TimeSlots { get; set; }
        
    }
    public class TimeSlots
    {
        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }
}
