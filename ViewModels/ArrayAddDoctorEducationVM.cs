using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{

    public class ArrayAddDoctorEducationVM
    {
        [Required]
        public DoctorEducationalViewModel[] ArrDoctorEducationalViewModel { get; set; }

        public string UserId { get; set; }
    }
    public class ArrayDoctorEducationalViewModel
    {
        [Required]
        public DoctorEducationalViewModel[] ArrDoctorEducationalViewModel { get; set; }
    }

    public class DoctorEducationalViewModel
    {
        [Required]
        public int DegreeId { get; set; }
        [Required]
        public string InstituteName { get; set; }
    }

    public class ArrayLanguageIdVM
    {
        public int[] languageId { get; set; }
        public string UserId { get; set; }
    }
    

    public class DoctorLanguageViewModel
    {
        [Required]
        public int[] languageId { get; set; } 
    }

    public class ArraySpecialityIdVM
    {
        [Required]
        public int[] SpecialityId { get; set; }
        [Required]
        public decimal Experience { get; set; }
    }

}
