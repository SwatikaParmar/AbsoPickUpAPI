using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class PatientPastMedicalHistoryViewModel 
    {
        public long Id { get; set; }
        public string TreatmentName { get; set; }
        public string DoctorName { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }

    }
}
