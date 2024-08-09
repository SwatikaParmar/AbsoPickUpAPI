using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UpdatePrescriptionViewModel
    {
        public string PrescriptionId { get; set; }
       
        public string MedicareNumber { get; set; }
        
        public string ReferenceNumber { get; set; }
       
        public string EntitlementNumber { get; set; }
        
        public bool ISPBSSafetyEntitlementCardHolder { get; set; }
      
        public bool IsPBSSafetyConcessionCardHolder { get; set; }
        
        public bool IsPBSPrescriptionFromStateManager { get; set; }
        
        public bool IsRBPSPrescription { get; set; }
        
        public bool IsBrandNotPermitted { get; set; }
        public List<PatientMedicationsViewModel> PatientMedicines { get; set; }

    }
}
