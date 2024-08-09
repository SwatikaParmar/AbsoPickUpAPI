using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AnfasAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AnfasAPI.Repositories
{
    public class ContentService : IContentService
    {
        private ApplicationDbContext _context;

        public ContentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ContentViewModel> GetCountries()
        {
            var countries = _context.CountryMaster.Select(
                                                   item => new ContentViewModel
                                                   {
                                                       Id = item.Id,
                                                       Name = item.Name
                                                   }).ToList();
            return countries;
        }

        public List<ContentViewModel> GetNationalities()
        {
            var nationalities = _context.NationalityMaster.Select(
                                                   item => new ContentViewModel
                                                   {
                                                       Id = item.Id,
                                                       Name = item.Name
                                                   }).ToList();
            return nationalities;
        }

        public List<ContentViewModel> GetStatesByCountryId(int CountryId)
        {
            return _context.StateMaster.Where(x => x.CountryId == CountryId).Select(
                                                   x => new ContentViewModel
                                                   {
                                                       Id = x.Id,
                                                       Name = x.Name
                                                   }).ToList();

        }

        public List<ContentViewModel> GetStates()
        {
            return _context.StateMaster.Select(
                                                   x => new ContentViewModel
                                                   {
                                                       Id = x.Id,
                                                       Name = x.Name
                                                   }).ToList();

        }

        public List<ContentDomainViewModel> GetLanguages()
        {
            return _context.LanguageMaster.Where(x => x.IsActive == true).Select(
                                                 x => new ContentDomainViewModel
                                                 {
                                                     Id = x.Id,
                                                     Name = x.Name,
                                                     IsActive = x.IsActive
                                                 }).OrderBy(x => x.Name).ToList();

        }

        public List<SpecialityContentDomainViewModel> GetSpecialities()
        {
            return _context.SpecialityMaster.Where(x => x.IsActive == true).Select(
                                                   x => new SpecialityContentDomainViewModel
                                                   {
                                                       Id = x.Id,
                                                       Name = x.Name,
                                                       Description = x.Description,
                                                       ImagePath = x.ImagePath,
                                                       IsActive = x.IsActive
                                                   }).OrderBy(x => x.Name).ToList();

        }

        public List<ContentDomainViewModel> GetDegree()
        {
            return _context.DegreeMaster.Where(x => x.IsActive == true).Select(
                                                                x => new ContentDomainViewModel
                                                                {
                                                                    Id = x.Id,
                                                                    Name = x.Name,
                                                                    IsActive = x.IsActive
                                                                }).OrderBy(x => x.Name).ToList();
        }

        public bool AddUpdateUserConfigurations(UserConfigurations _userConfigurations)
        {
            try
            {
                var model = _context.UserConfigurations.Where(x => x.UserId == _userConfigurations.UserId).FirstOrDefault();
                if (model != null)
                {
                    model.PushNotificationsStatus = _userConfigurations.PushNotificationsStatus;
                    model.EmailNotificationStatus = _userConfigurations.EmailNotificationStatus;
                    model.SMSNotificationStatus = _userConfigurations.SMSNotificationStatus;
                    model.AppLanguageId = _userConfigurations.AppLanguageId;
                    _context.UserConfigurations.Update(model);
                }
                else
                {
                    _context.Add(_userConfigurations);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public GetUserConfigurationsViewModel GetUserConfigurations(string UserId)
        {
            var _userConfigurations = _context.UserConfigurations.Where(x => x.UserId == UserId)
                                                                 .Select(
                                                                     x => new GetUserConfigurationsViewModel
                                                                     {
                                                                         Id = x.Id,
                                                                         UserId = x.UserId,
                                                                         PushNotificationStatus = x.PushNotificationsStatus,
                                                                         SMSNotificationStatus = x.SMSNotificationStatus,
                                                                         EmailNotificationStatus = x.EmailNotificationStatus
                                                                     }).FirstOrDefault();
            return _userConfigurations;


        }

        public bool AddUpdateAddressInfo(UserAddress _userAddress)
        {
            try
            {
                var model = _context.UserAddress.Where(x => x.UserId == _userAddress.UserId).FirstOrDefault();
                if (model != null)
                {
                    model.NationalityId = _userAddress.NationalityId;
                    model.CountryId = _userAddress.CountryId;
                    model.StateId = _userAddress.StateId;
                    model.City = _userAddress.City;
                    model.CurrentAddress = _userAddress.CurrentAddress;
                    _context.UserAddress.Update(model);
                }
                else
                {
                    _context.UserAddress.Add(_userAddress);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<AppSettingsViewModel> GetAppSettings()
        {
            return await _context.AppSettings.Where(x => x.IsDeleted == false).Select(
                                                 x => new AppSettingsViewModel
                                                 {
                                                     Id = x.Id,
                                                     AboutUs = x.AboutUs,
                                                     PrivacyPolicy = x.PrivacyPolicy,
                                                     TermsConditions = x.TermsConditions
                                                 }).FirstOrDefaultAsync();


        }
        public bool AddUpdateAppSettings(AppSettings _appSettings)
        {
            try
            {
                var model = _context.AppSettings.Where(x => x.Id == _appSettings.Id).FirstOrDefault();
                if (model != null)
                {
                    model.AboutUs = _appSettings.AboutUs;
                    model.PrivacyPolicy = _appSettings.PrivacyPolicy;
                    model.TermsConditions = _appSettings.TermsConditions;
                    _context.AppSettings.Update(model);
                }
                else
                {
                    _context.Add(_appSettings);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddUpdateLanguageMaster(LanguageMaster _languageMaster)
        {
            try
            {
                var model = _context.LanguageMaster.Where(x => x.Id == _languageMaster.Id).FirstOrDefault();
                if (model != null)
                {
                    model.Name = _languageMaster.Name;
                    model.IsActive = _languageMaster.IsActive;
                    _context.LanguageMaster.Update(model);
                }
                else
                {
                    _context.Add(_languageMaster);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddUpdateDegreeMaster(DegreeMaster _degreeMaster)
        {
            try
            {
                var model = _context.DegreeMaster.Where(x => x.Id == _degreeMaster.Id).FirstOrDefault();
                if (model != null)
                {
                    model.Name = _degreeMaster.Name;
                    model.IsActive = _degreeMaster.IsActive;
                    _context.DegreeMaster.Update(model);
                }
                else
                {
                    _context.Add(_degreeMaster);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddUpdateSpecialityMaster(SpecialityMaster _specialityMaster)
        {
            try
            {
                var model = _context.SpecialityMaster.Where(x => x.Id == _specialityMaster.Id).FirstOrDefault();
                if (model != null)
                {
                    model.Name = _specialityMaster.Name;
                    model.ImagePath = !string.IsNullOrEmpty(_specialityMaster.ImagePath) ? _specialityMaster.ImagePath : model.ImagePath;
                    model.IsActive = _specialityMaster.IsActive;
                    _context.SpecialityMaster.Update(model);
                }
                else
                {
                    _context.Add(_specialityMaster);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateNotificationStatus(string userId, NotificatonStatusViewModel _notificationStatus)
        {
            try
            {
                var model = _context.UserConfigurations.Where(x => x.UserId == userId).FirstOrDefault();
                if (model != null)
                {
                    if (_notificationStatus.EmailNotificationStatus != null)
                    {
                        model.EmailNotificationStatus = _notificationStatus.EmailNotificationStatus.Value;
                    }
                    if (_notificationStatus.SMSNotificationStatus != null)
                    {
                        model.SMSNotificationStatus = _notificationStatus.SMSNotificationStatus.Value;
                    }
                    _context.UserConfigurations.Update(model);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateUserAddress(UserAddress _userAddress)
        {
            try
            {
                var model = _context.UserAddress.Where(x => x.UserId == _userAddress.UserId).FirstOrDefault();
                if (model != null)
                {
                    model.CountryId = _userAddress.CountryId;
                    model.StateId = _userAddress.StateId;
                    model.City = _userAddress.City;
                    model.NationalityId = _userAddress.NationalityId;
                    model.CurrentAddress = _userAddress.CurrentAddress;
                    _context.UserAddress.Update(model);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}

