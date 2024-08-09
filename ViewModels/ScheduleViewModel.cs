using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ScheduleViewModel
    {
        public Schedule[] schedule { get; set; }
        
    }
    public class DocScheduleVM
    {
        public string UserId { get; set; }
        public bool IsAvailable { get; set; }
        public Schedule[] schedule { get; set; }
    }

    public class Schedule
    {
        public timeShifts TimeShift { get; set; }
        public timeSlots[] TimeSlots { get; set; }
    }
    public class timeSlots
    {
        public string SlotStart { get; set; }
        public string SlotEnd { get; set; }
    }
    public class timeShifts
    {
        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }
}
