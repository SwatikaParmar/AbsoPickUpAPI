using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class AppointmentStatusChangeViewModel
    {
        [Required]
        public string AppointmentId { get; set; }

        [Required]
        public int AppointmentStatus { get; set; }
    }
}
