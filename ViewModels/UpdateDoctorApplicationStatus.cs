using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UpdateDoctorApplicationStatus
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ApplicationStatus { get; set; }
    }
}
