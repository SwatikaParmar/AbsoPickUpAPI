using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IAuthService
    {
        ApplicationUser GetById(string userId);
        Task<ApplicationUser> GetUserBySocialIdAndType(string socialId, string loginType);
        Task<PatientLoginResponseViewModel> GetPatientLoginResponse(string userId, string accessToken);
        Task<PatientProfileResponseViewModel> GetPatientProfileResponse(string userId);
        Task<DoctorLoginResponseViewModel> GetDoctorLoginResponse(string userId, string accessToken);
        int GetDoctorProfileCompletePersentById(string userId);
        int GetPatientProfileCompletePersentById(string userId);
    }
}
