using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IDoctorService
    {
        bool AddDoctorLanguage(ArrayLanguageIdVM request); 
        bool AddDoctorDegree(ArrayAddDoctorEducationVM request);
        bool UpdateDoctorSpeciality(DoctorSpecialityViewModel request);
        Task<DoctorBasicInfoViewModel> GetDoctorPersonalInfo(string userId);
        Task<DoctorWorkInfoViewModel> GetDoctorWorkInfo(string userId);
        bool AddDoctorBankInfo(DoctorBankInfo _doctorBankInfo);
        DoctorBankInfo GetDoctorBankInfo(string userId);
        FilterationResponseModel<AdminDoctorsListViewModel> GetAdminDoctorsList(FilterationListViewModel model);
        Task<DoctorProfileDetailsViewModel> GetDoctorDetailInfo(string doctorId);
        bool UpdateDoctorApplicationStatus(UpdateDoctorApplicationStatus model);
        Task<DoctorDashboardInfoViewModel> GetDoctorDashboardInfo(string userId);
        FilterationResponseModel <GetTopDoctorsViewModel> GetTopDoctors(FilterationListViewModel model);
        FilterationResponseModel<GetTopDoctorsViewModel> GetAllDoctors(FilterationListViewModel model);

        Task<GetDoctorDetailsViewModel> GetDoctorDescription(string doctorId);
    }
}
