using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IPatientService
    {
        Task<PatientBasicInfoViewModel> GetPatientBasicInfo(string userId);
        Task<PatientAdditionalInfoVM> GetPatientAdditionalInfo(string userId);
        bool AddUpdatePatientHealthInfo(PatientHealthInfo _patientHealthInfo);
        FilterationResponseModel<AdminPatientsListViewModel> GetAdminPatientsList(FilterationListViewModel model);
        Task<PatientDashboardInfoViewModel> GetPatientDashboardInfo(string userId);
        Task<GetPatientInfoViewModel> GetPatientInfoById(string PatientId);
    }
}
