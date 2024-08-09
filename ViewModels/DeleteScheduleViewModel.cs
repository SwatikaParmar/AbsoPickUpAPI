using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DeleteScheduleViewModel
    {
        [Required]
        public string Date { get; set; }
        [Required]
        public TimeSlots TimeSlots { get; set; }
    }
}
