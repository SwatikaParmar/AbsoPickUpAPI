using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class GetPrescriptionListViewModel
    {
        public string PrescriptionId { get; set; }
        public string AppointmentId { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string MediCareNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string EntitlementNumber { get; set; }
        public bool IsPBSSafetyEntitlementCardHolder { get; set; }
        public bool IsBrandNotPermitted { get; set; }
        public bool IsPBSPrescriptionFromStateManager { get; set; }
        public bool IsPBSSafetyConcessionCardHolder { get; set; }
        public bool IsRBPSPrescription { get; set; }
        public List<GetPatientMedicines> Medicines { get; set; }
       
    }
    public class GetPatientMedicines
    {
        public string MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string Quantity { get; set; }
        public string DosageDirections { get; set; }
        public string NumberOfRepeats { get; set; }
        public string  Date { get; set; }
    }
   
}
