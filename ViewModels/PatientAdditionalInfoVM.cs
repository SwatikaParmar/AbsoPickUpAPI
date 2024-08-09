using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientAdditionalInfoVM
    { 
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string BloodGroup { get; set; }
        public bool IsVegetarian { get; set; }
        public bool UseAlcohol { get; set; }
        public bool UseSmoke { get; set; }
        public bool UseDrug { get; set; }
    }
}
