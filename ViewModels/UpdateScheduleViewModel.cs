using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UpdateScheduleViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        // public TimeSlots  NewTimeSlots{ get; set; }
        public Schedule schedule { get; set; }

    }
}
