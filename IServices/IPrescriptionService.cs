using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IPrescriptionService
    {
        ResponseStatusIdViewModel AddPrescriptions(Prescriptions _Prescriptions);
        bool AddPatientMedications(Medications model);
        List<GetPrescriptionListViewModel> GetPrescriptionList(string UserId, PrescriptionListViewModel model);
        bool UpdatePrescriptionDetails(string userId, UpdatePrescriptionViewModel model);
        Task<bool> DeleteMedicationById(string medicineId);


    }
}
