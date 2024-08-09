using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class PatientService : IPatientService
    {
        private ApplicationDbContext _context;
        private IAuthService _authService;
        public PatientService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<PatientBasicInfoViewModel> GetPatientBasicInfo(string userId)
        {
            return await (from u in _context.ApplicationUser

                        join gm in _context.GenderMaster
                        on u.GenderId equals gm.Id into gmGroup
                        from gm in gmGroup.DefaultIfEmpty()

                        join ua in _context.UserAddress
                        on u.Id equals ua.UserId into uaGroup
                        from ua in uaGroup.DefaultIfEmpty()

                        join cm in _context.CountryMaster
                        on ua.CountryId equals cm.Id into cmGroup
                        from cm in cmGroup.DefaultIfEmpty()

                        join sm in _context.StateMaster
                        on ua.StateId equals sm.Id into smGroup
                        from sm in smGroup.DefaultIfEmpty()
                        where u.Id == userId
                          select new PatientBasicInfoViewModel
                          {
                              FirstName = (!string.IsNullOrEmpty(u.FirstName)) ? u.FirstName : string.Empty,
                              LastName = (!string.IsNullOrEmpty(u.LastName)) ? u.LastName : string.Empty,
                              DateOfBirth = (u.DateOfBirth != null) ? u.DateOfBirth.Value.ToString(GlobalVariables.DefaultDateFormat) : "",
                              Email = u.Email,
                              IsEmailVerified = u.EmailConfirmed,
                              DialCode = (!string.IsNullOrEmpty(u.DialCode)) ? u.DialCode : string.Empty,
                              PhoneNumber = (!string.IsNullOrEmpty(u.PhoneNumber)) ? u.PhoneNumber : string.Empty,
                              IsPhoneNoVerified = u.PhoneNumberConfirmed,
                              GenderId = (gm.Id > 0) ? gm.Id : 0,
                              Gender = (!string.IsNullOrEmpty(gm.Name)) ? gm.Name : string.Empty,
                              ProfilePic = (!string.IsNullOrEmpty(u.ProfilePic)) ? u.ProfilePic : string.Empty,
                              City = (!string.IsNullOrEmpty(ua.City)) ? ua.City : string.Empty,
                              StateId = (sm.Id > 0) ? sm.Id : 0,
                              State = (!string.IsNullOrEmpty(sm.Name)) ? sm.Name : string.Empty,
                              CountryId = (cm.Id > 0) ? cm.Id : 0,
                              Country = (!string.IsNullOrEmpty(cm.Name)) ? cm.Name : string.Empty,
                              CurrentAddress = (!string.IsNullOrEmpty(ua.CurrentAddress)) ? ua.CurrentAddress : string.Empty

                          }).FirstOrDefaultAsync();

        }

        public async Task<PatientAdditionalInfoVM> GetPatientAdditionalInfo(string userId)
        {
            try
            {
                return await _context.PatientHealthInfo.Where(x => x.UserId == userId).Select(
                                                                        x => new PatientAdditionalInfoVM
                                                                        {
                                                                            Height = x.Height,
                                                                            Weight = x.Weight,
                                                                            BloodGroup = (!string.IsNullOrEmpty(x.BloodGroup)) ? x.BloodGroup : string.Empty,
                                                                            IsVegetarian = x.IsVegetarian,
                                                                            UseAlcohol = x.UseAlcohol,
                                                                            UseDrug = x.UseDrug,
                                                                            UseSmoke = x.UseSmoke
                                                                        }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool AddUpdatePatientHealthInfo(PatientHealthInfo _patientHealthInfo)
        {
            try
            {
                var model = _context.PatientHealthInfo.Where(x => x.UserId == _patientHealthInfo.UserId).FirstOrDefault();
                if (model != null)
                {
                    model.Height = _patientHealthInfo.Height;
                    model.Weight = _patientHealthInfo.Weight;
                    model.BloodGroup = _patientHealthInfo.BloodGroup;
                    model.UseSmoke = _patientHealthInfo.UseSmoke;
                    model.UseDrug = _patientHealthInfo.UseDrug;
                    model.IsVegetarian = _patientHealthInfo.IsVegetarian;
                    model.UseAlcohol = _patientHealthInfo.UseAlcohol;
                    _context.PatientHealthInfo.Update(model);
                }
                else
                {
                    _context.Add(_patientHealthInfo);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public FilterationResponseModel<AdminPatientsListViewModel> GetAdminPatientsList(FilterationListViewModel model)
        {
            if (_context != null)
            {
                var source = (from u in _context.ApplicationUser
                              join ur in _context.UserRoles
                              on u.Id equals ur.UserId
                              join r in _context.Roles
                              on ur.RoleId equals r.Id
                              join g in _context.GenderMaster
                              on u.GenderId equals g.Id
                              where r.Name.ToLower() == GlobalVariables.UserRole.Patient.ToString().ToLower()
                              && (u.IsDeleted == false)
                              select new AdminPatientsListViewModel
                              {
                                  Id = u.Id,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Email = u.Email,
                                  IsEmailVerified = u.EmailConfirmed,
                                  DialCode = u.DialCode,
                                  PhoneNumber = u.PhoneNumber,
                                  IsPhoneNoVerified = u.PhoneNumberConfirmed,
                                  GenderId = u.GenderId,
                                  Gender = g.Name
                              }).AsQueryable();


                // searching
                if (!string.IsNullOrWhiteSpace(model.searchQuery))
                {
                    var search = model.searchQuery.ToLower();
                    source = source.Where(x =>
                                                x.FirstName.ToLower().Contains(search) ||
                                                x.LastName.ToLower().Contains(search) ||
                                                x.Email.ToLower().Contains(search) ||
                                                x.PhoneNumber.ToLower().Contains(search) ||
                                                x.Gender.ToLower().Contains(search)
                                                );
                }

                // Get's No of Rows Count   
                int count = source.Count();

                // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                int CurrentPage = model.pageNumber;

                // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                int PageSize = model.pageSize;

                // Display TotalCount to Records to User  
                int TotalCount = count;

                // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                // Returns List of Customer after applying Paging   
                var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                // if CurrentPage is greater than 1 means it has previousPage  
                var previousPage = CurrentPage > 1 ? "Yes" : "No";

                // if TotalPages is greater than CurrentPage means it has nextPage  
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                // Returing List of Customers Collections  
                FilterationResponseModel<AdminPatientsListViewModel> obj = new FilterationResponseModel<AdminPatientsListViewModel>();
                obj.totalCount = TotalCount;
                obj.pageSize = PageSize;
                obj.currentPage = CurrentPage;
                obj.totalPages = TotalPages;
                obj.previousPage = previousPage;
                obj.nextPage = nextPage;
                obj.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                obj.dataList = items.ToList();
                return obj;
            }
            return null;
        }

        public async Task<PatientDashboardInfoViewModel> GetPatientDashboardInfo(string userId)
        {
            return await _context.ApplicationUser.Where(x => x.Id == userId)
                         .Select(x => new PatientDashboardInfoViewModel
                         {
                             FirstName = x.FirstName,
                             LastName = x.LastName,
                             PercentageProfileComplete = _authService.GetPatientProfileCompletePersentById(userId)
                         }).FirstOrDefaultAsync();
        }

        public async Task<GetPatientInfoViewModel> GetPatientInfoById(string PatientId)
        {
            return await (from u in _context.ApplicationUser

                          join gm in _context.GenderMaster
                          on u.GenderId equals gm.Id into gmGroup
                          from gm in gmGroup.DefaultIfEmpty()

                          join ph in _context.PatientHealthInfo
                          on u.Id equals ph.UserId into phGroup
                          from ph in phGroup.DefaultIfEmpty()

                          join ua in _context.UserAddress
                          on u.Id equals ua.UserId into uaGroup
                          from ua in uaGroup.DefaultIfEmpty()

                          join cm in _context.CountryMaster
                          on ua.CountryId equals cm.Id into cmGroup
                          from cm in cmGroup.DefaultIfEmpty()

                          join sm in _context.StateMaster
                          on ua.StateId equals sm.Id into smGroup
                          from sm in smGroup.DefaultIfEmpty()
                          where u.Id == PatientId
                          select new GetPatientInfoViewModel
                          {
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              DateOfBirth = (u.DateOfBirth != null) ? u.DateOfBirth.Value.ToString(GlobalVariables.DefaultDateFormat) : "",
                              Email = u.Email,
                              IsEmailVerified = u.EmailConfirmed,
                              DialCode = u.DialCode,
                              PhoneNumber = u.PhoneNumber,
                              IsPhoneNoVerified = u.PhoneNumberConfirmed,
                              GenderId = (gm.Id > 0) ? gm.Id : 0,
                              Gender = (!string.IsNullOrEmpty(gm.Name)) ? gm.Name : string.Empty,
                              ProfilePic = (!string.IsNullOrEmpty(u.ProfilePic)) ? u.ProfilePic : string.Empty,
                              City = (!string.IsNullOrEmpty(ua.City)) ? ua.City : string.Empty,
                              StateId = (sm.Id > 0) ? sm.Id : 0,
                              State = (!string.IsNullOrEmpty(sm.Name)) ? sm.Name : string.Empty,
                              CountryId = (cm.Id > 0) ? cm.Id : 0,
                              Country = (!string.IsNullOrEmpty(cm.Name)) ? cm.Name : string.Empty,
                              CurrentAddress = (!string.IsNullOrEmpty(ua.CurrentAddress)) ? ua.CurrentAddress : string.Empty,
                              Height = (ph.Height > 0) ? ph.Height : 0,
                              Weight = (ph.Weight > 0) ? ph.Weight : 0,
                              BloodGroup = (!string.IsNullOrEmpty(ph.BloodGroup)) ? ph.BloodGroup : string.Empty,
                              IsVegetarian = (ph.IsVegetarian == true) ? ph.IsVegetarian : false,
                              UseAlcohol = (ph.UseAlcohol == true) ? ph.UseAlcohol : false,
                              UseSmoke = (ph.UseSmoke == true) ? ph.UseSmoke : false,
                              UseDrug = (ph.UseDrug == true) ? ph.UseDrug : false,
                          }).FirstOrDefaultAsync();
        }
    }
}
