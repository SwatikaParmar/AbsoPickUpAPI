using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AvailableTimeShiftViewModel
    {
        public string Date { get; set; }
        public List<ShiftViewModel> AvailableShifts { get; set; }
        
    }
    public class ShiftViewModel
    { 
        public int Id { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }

}
