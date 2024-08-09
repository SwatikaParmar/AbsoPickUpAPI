using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UserRatingViewModel
    {
        [Required]
        public string AppointmentId { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }

    }
}
