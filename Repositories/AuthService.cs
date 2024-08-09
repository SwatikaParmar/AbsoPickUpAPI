using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class AuthService : IAuthService
    {
        private ApplicationDbContext _context;
        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetById(string id)
        {
            return _context.ApplicationUser.Find(id);
        }

        public async Task<ApplicationUser> GetUserBySocialIdAndType(string socialId, string loginType)
        {
            if (loginType.ToLower() == GlobalVariables.LoginType.Facebook.ToString().ToLower())
            {
                return await _context.ApplicationUser.Where(x => x.FacebookId == socialId).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.ApplicationUser.Where(x => x.GoogleId == socialId).FirstOrDefaultAsync();
            }
        }

        public async Task<PatientLoginResponseViewModel> GetPatientLoginResponse(string userId, string accessToken)
        {
            return await (from u in _context.ApplicationUser
                          join ua in _context.UserAddress
                          on u.Id equals ua.UserId into uaGroup
                          from ua in uaGroup.DefaultIfEmpty()
                          join cm in _context.CountryMaster
                          on ua.CountryId equals cm.Id into cmGroup
                          from cm in cmGroup.DefaultIfEmpty()
                          join sm in _context.StateMaster
                          on ua.StateId equals sm.Id into smGroup
                          from sm in smGroup.DefaultIfEmpty()
                          join uc in _context.UserConfigurations
                          on u.Id equals uc.UserId into ucGroup
                          from uc in ucGroup.DefaultIfEmpty()
                          where u.Id == userId
                          select new PatientLoginResponseViewModel
                          {
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              DateOfBirth = (u.DateOfBirth != null) ? u.DateOfBirth.Value.ToString(GlobalVariables.DefaultDateFormat) : "",
                              Email = u.Email,
                              IsEmailVerified = u.EmailConfirmed,
                              DialCode = (!string.IsNullOrEmpty(u.DialCode)) ? u.DialCode : string.Empty,
                              PhoneNumber = (!string.IsNullOrEmpty(u.PhoneNumber)) ? u.PhoneNumber : string.Empty,
                              IsPhoneNoVerified = u.PhoneNumberConfirmed,
                              Gender = u.GenderId,
                              IsSocialUser = u.IsSocialUser,
                              ProfilePic = (!string.IsNullOrEmpty(u.ProfilePic)) ? u.ProfilePic : string.Empty,
                              City = (!string.IsNullOrEmpty(ua.City)) ? ua.City : string.Empty,
                              State = (!string.IsNullOrEmpty(sm.Name)) ? sm.Name : string.Empty,
                              Country = (!string.IsNullOrEmpty(cm.Name)) ? cm.Name : string.Empty,
                              CurrentAddress = (!string.IsNullOrEmpty(ua.CurrentAddress)) ? ua.CurrentAddress : string.Empty,
                              EmailNotificationStatus = uc.EmailNotificationStatus,
                              PercentageProfileComplete = GetPatientProfileCompletePersentById(userId),
                              SMSNotificationStatus = uc.SMSNotificationStatus,
                              AccessToken = accessToken
                          }).FirstOrDefaultAsync();

        }

        public async Task<PatientProfileResponseViewModel> GetPatientProfileResponse(string userId)
        {
            return await (from u in _context.ApplicationUser
                          join ua in _context.UserAddress
                          on u.Id equals ua.UserId into uaGroup
                          from ua in uaGroup.DefaultIfEmpty()
                          join cm in _context.CountryMaster
                          on ua.CountryId equals cm.Id into cmGroup
                          from cm in cmGroup.DefaultIfEmpty()
                          join sm in _context.StateMaster
                          on ua.StateId equals sm.Id into smGroup
                          from sm in smGroup.DefaultIfEmpty()
                          join uc in _context.UserConfigurations
                          on u.Id equals uc.UserId into ucGroup
                          from uc in ucGroup.DefaultIfEmpty()
                          where u.Id == userId
                          select new PatientProfileResponseViewModel
                          {
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              DateOfBirth = (u.DateOfBirth != null) ? u.DateOfBirth.Value.ToString(GlobalVariables.DefaultDateFormat) : "",
                              Email = u.Email,
                              IsEmailVerified = u.EmailConfirmed,
                              DialCode = (!string.IsNullOrEmpty(u.DialCode)) ? u.DialCode : string.Empty,
                              PhoneNumber = (!string.IsNullOrEmpty(u.PhoneNumber)) ? u.PhoneNumber : string.Empty,
                              IsPhoneNoVerified = u.PhoneNumberConfirmed,
                              Gender = u.GenderId,
                              IsSocialUser = u.IsSocialUser,
                              ProfilePic = (!string.IsNullOrEmpty(u.ProfilePic)) ? u.ProfilePic : string.Empty,
                              City = (!string.IsNullOrEmpty(ua.City)) ? ua.City : string.Empty,
                              State = (!string.IsNullOrEmpty(sm.Name)) ? sm.Name : string.Empty,
                              Country = (!string.IsNullOrEmpty(cm.Name)) ? cm.Name : string.Empty,
                              CurrentAddress = (!string.IsNullOrEmpty(ua.CurrentAddress)) ? ua.CurrentAddress : string.Empty,
                              EmailNotificationStatus = uc.EmailNotificationStatus,
                              PercentageProfileComplete = GetPatientProfileCompletePersentById(userId),
                              SMSNotificationStatus = uc.SMSNotificationStatus

                          }).FirstOrDefaultAsync();

        }

        public async Task<DoctorLoginResponseViewModel> GetDoctorLoginResponse(string userId, string accessToken)
        {
            return await (from u in _context.ApplicationUser
                          join ua in _context.UserAddress
                          on u.Id equals ua.UserId into uaGroup
                          from ua in uaGroup.DefaultIfEmpty()
                          join cm in _context.CountryMaster
                          on ua.CountryId equals cm.Id into cmGroup
                          from cm in cmGroup.DefaultIfEmpty()
                          join sm in _context.StateMaster
                          on ua.StateId equals sm.Id into smGroup
                          from sm in smGroup.DefaultIfEmpty()
                          join uc in _context.UserConfigurations
                          on u.Id equals uc.UserId into ucGroup
                          from uc in ucGroup.DefaultIfEmpty()
                          where u.Id == userId
                          select new DoctorLoginResponseViewModel
                          {
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              DateOfBirth = (u.DateOfBirth != null) ? u.DateOfBirth.Value.ToString(GlobalVariables.DefaultDateFormat) : "",
                              Email = u.Email,
                              //IsEmailVerified = u.EmailConfirmed,
                              DialCode = (!string.IsNullOrEmpty(u.DialCode)) ? u.DialCode : string.Empty,
                              PhoneNumber = (!string.IsNullOrEmpty(u.PhoneNumber)) ? u.PhoneNumber : string.Empty,
                              //IsPhoneNoVerified = u.PhoneNumberConfirmed,
                              Gender = u.GenderId,
                              IsSocialUser = u.IsSocialUser,
                              ProfilePic = (!string.IsNullOrEmpty(u.ProfilePic)) ? u.ProfilePic : string.Empty,
                              City = (!string.IsNullOrEmpty(ua.City)) ? ua.City : string.Empty,
                              State = (!string.IsNullOrEmpty(sm.Name)) ? sm.Name : string.Empty,
                              Country = (!string.IsNullOrEmpty(cm.Name)) ? cm.Name : string.Empty,
                              CurrentAddress = (!string.IsNullOrEmpty(ua.CurrentAddress)) ? ua.CurrentAddress : string.Empty,
                              EmailNotificationStatus = uc.EmailNotificationStatus,
                              SMSNotificationStatus = uc.SMSNotificationStatus,
                              LastScreenId = u.LastScreenId,
                              PercentageProfileComplete = GetDoctorProfileCompletePersentById(userId),
                              ApplicationStatus = u.ApplicationStatus,
                              AccessToken = accessToken,
                              //Otp = u.Otp.Value
                          }).FirstOrDefaultAsync();
        }

        public int GetDoctorProfileCompletePersentById(string userId)
        {
            int profileCompletePersent = 0;
            var userDetails = _context.ApplicationUser.Find(userId);
            if (userDetails != null)
            {
                profileCompletePersent = profileCompletePersent + 10;
                if (userDetails.EmailConfirmed)
                {
                    profileCompletePersent = profileCompletePersent + 10;
                }

                if (userDetails.PhoneNumberConfirmed)
                {
                    profileCompletePersent = profileCompletePersent + 10;
                }

                if (!string.IsNullOrEmpty(userDetails.RegNo))
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }

                if (!string.IsNullOrEmpty(userDetails.About))
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }
            }

            // doctor education info
            var userEducationInfo = _context.DoctorEducationInfo.Any(x => x.UserId == userId);
            if (userEducationInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
            }

            // doctor Language info
            var userLanguageInfo = _context.DoctorLanguageInfo.Any(x => x.UserId == userId);
            if (userLanguageInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
            }

            // doctor Speciality info
            var userSpecialityInfo = _context.DoctorSpecialityInfo.Any(x => x.UserId == userId);
            if (userSpecialityInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
            }

            // doctor Address info
            var userAddressInfo = _context.UserAddress.Any(x => x.UserId == userId);
            if (userAddressInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
            }

            // doctor Configuration info
            var userConfigurationsInfo = _context.UserConfigurations.Any(x => x.UserId == userId);
            if (userConfigurationsInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
            }

            // doctor Wallet Billing Info
            var userWalletBillingInfo = _context.WalletBillingInfo.Any(x => x.UserId == userId);
            if (userWalletBillingInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
            }

            return profileCompletePersent;
        }

        public int GetPatientProfileCompletePersentById(string userId)
        {
            int profileCompletePersent = 0;
            var userDetails = _context.ApplicationUser.Find(userId);
            if (userDetails != null)
            {
                profileCompletePersent = profileCompletePersent + 10;
                if (userDetails.EmailConfirmed)
                {
                    profileCompletePersent = profileCompletePersent + 10;
                }

                if (userDetails.PhoneNumberConfirmed)
                {
                    profileCompletePersent = profileCompletePersent + 10;
                }

                if (!string.IsNullOrEmpty(userDetails.ProfilePic))
                {
                    profileCompletePersent = profileCompletePersent + 10;
                }
            }

            // patient Health Info
            var userHealthInfo = _context.PatientHealthInfo.Where(x => x.UserId == userId).FirstOrDefault();
            if (userHealthInfo != null)
            {
                profileCompletePersent = profileCompletePersent + 5;
                if (userHealthInfo.Height > 0)
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }

                if (userHealthInfo.Weight > 0)
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }

                if (!string.IsNullOrEmpty(userHealthInfo.BloodGroup))
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }
            }

            // patient Address info
            var userAddressInfo = _context.UserAddress.Any(x => x.UserId == userId);
            if (userAddressInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
                var addressInfo = _context.UserAddress.Where(x => x.UserId == userId).FirstOrDefault();

                if (addressInfo.CountryId > 0)
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }

                if (addressInfo.StateId > 0)
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }

                if (!string.IsNullOrEmpty(addressInfo.City))
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }

                if (!string.IsNullOrEmpty(addressInfo.CurrentAddress))
                {
                    profileCompletePersent = profileCompletePersent + 5;
                }
            }

            // patient Configuration info
            var userConfigurationsInfo = _context.UserConfigurations.Any(x => x.UserId == userId);
            if (userConfigurationsInfo)
            {
                profileCompletePersent = profileCompletePersent + 10;
            }

            return profileCompletePersent;
        }
    }
}
