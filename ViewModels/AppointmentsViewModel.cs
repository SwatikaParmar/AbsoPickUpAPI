using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AppointmentsViewModel
    {
        [Required]
        public long TimeSlotId { get; set; }

        [Required]
        public string DoctorId { get; set; }
    }
}
