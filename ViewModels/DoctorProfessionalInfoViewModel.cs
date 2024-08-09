using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class DoctorProfessionalInfoViewModel
    {
        [Required]
        public int[] SpecialityId { get; set; }
        [Required]
        public int NationalityId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(20)]
        public string RegNo { get; set; }
    }
}
