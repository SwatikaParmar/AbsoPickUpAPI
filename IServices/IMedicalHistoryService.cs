using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IMedicalHistoryService
    {
        bool AddPatientMedicalHistory(PatientPastMedicalHistory __patientMedicalHistoryModel);
        Task<bool> DeleteMedicalHistoryById(int MedicalHistoryId, string UserId);
        List<PatientPastMedicalHistoryViewModel> GetPatientPastMedicalHistory(string UserId);

        bool AddPatientSurgicalHistory(PatientSurgicalHistory _patientSurgicalHistory);
        Task<bool> DeleteSurgicalHistoryById(int surgicalHistoryId, string UserId);
        List<GetSurgicalHistoryViewModel> GetPatientSurgicalHistory(string UserId);

        bool AddPatientFamilyHistory(PatientFamilyHistory _patientFamilyHistory);
        List<GetFamilyHistoryViewModel> GetPatientFamilyHistory(string UserId);
        Task<bool> DeleteFamilyHistoryById(int familyHistoryId, string UserId);

        bool AddPatientAllergyHistory(PatientPastAllergyHistory _patientAllergyHistory);
        List<GetAllergyHistoryViewModel> GetPatientAllergyHistory(string UserId);
        Task<bool> DeleteAllergyHistoryById(int allergyHistoryId, string UserId);

        bool AddPatientMedicalReport(PatientMedicalReport _patientMedicalReport);
        
        Task<bool> DeletePatientMedicalReport(int medicalReportId, string UserId);
        List<GetMedicalReportViewModel> GetPatientMedicalReport(string UserId);




    }
}
