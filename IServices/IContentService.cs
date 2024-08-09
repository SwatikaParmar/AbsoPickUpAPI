using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IContentService
    {
        List<ContentViewModel> GetCountries();
        List<ContentViewModel> GetNationalities();
        List<ContentViewModel> GetStatesByCountryId(int CountryId);
         List<ContentViewModel> GetStates();
        List<ContentDomainViewModel> GetLanguages();
        bool AddUpdateLanguageMaster(LanguageMaster _languageMaster);
        List<SpecialityContentDomainViewModel> GetSpecialities();

        bool AddUpdateSpecialityMaster(SpecialityMaster _specialityMaster);
        List<ContentDomainViewModel> GetDegree();

        bool AddUpdateDegreeMaster(DegreeMaster _degreeMaster);
        bool AddUpdateUserConfigurations(UserConfigurations _userConfigurations);

        GetUserConfigurationsViewModel GetUserConfigurations(string UserId);
        bool AddUpdateAddressInfo(UserAddress _userAddress); 
        bool UpdateNotificationStatus(string userId, NotificatonStatusViewModel _notificationStatus);
        Task<AppSettingsViewModel> GetAppSettings();
        bool AddUpdateAppSettings(AppSettings _appSettings);
        bool UpdateUserAddress(UserAddress _userAddress);
        
    }
}
