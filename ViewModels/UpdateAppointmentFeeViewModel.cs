using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UpdateAppointmentFeeViewModel
    {
        [Required]
        public decimal Fee { get; set; }
    }
}
