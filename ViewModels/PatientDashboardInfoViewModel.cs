using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientDashboardInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PercentageProfileComplete { get; set; }
    }
}
