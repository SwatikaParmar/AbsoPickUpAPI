using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class DoctorService : IDoctorService
    {
        private ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IAppointmentService _appointmentService;

        public DoctorService(
            ApplicationDbContext context,
            IAuthService authService,
            IAppointmentService appointmentService
        )
        {
            _context = context;
            _authService = authService;
            _appointmentService = appointmentService;
        }

        public bool AddDoctorLanguage(ArrayLanguageIdVM request)
        {
            try
            {
                var itemList = _context.DoctorLanguageInfo
                    .Where(x => x.UserId == request.UserId)
                    .ToList();
                if (itemList.Count > 0)
                {
                    foreach (var item in itemList)
                    {
                        _context.DoctorLanguageInfo.Remove(item);
                    }
                    _context.SaveChanges();
                }

                foreach (var item in request.languageId)
                {
                    DoctorLanguageInfo doctorlanguage = new DoctorLanguageInfo();
                    doctorlanguage.UserId = request.UserId;
                    doctorlanguage.LanguageId = item;
                    _context.DoctorLanguageInfo.Add(doctorlanguage);
                    _context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateDoctorSpeciality(DoctorSpecialityViewModel model)
        {
            try
            {
                var itemList = _context.DoctorSpecialityInfo
                    .Where(x => x.UserId == model.UserId)
                    .ToList();
                if (itemList.Count > 0)
                {
                    foreach (var item in itemList)
                    {
                        _context.DoctorSpecialityInfo.Remove(item);
                    }
                    _context.SaveChanges();
                }

                foreach (var id in model.SpecialityId)
                {
                    DoctorSpecialityInfo doctorspeciality = new DoctorSpecialityInfo();
                    doctorspeciality.UserId = model.UserId;
                    doctorspeciality.SpecialityId = id;
                    _context.DoctorSpecialityInfo.Add(doctorspeciality);
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddDoctorDegree(ArrayAddDoctorEducationVM request)
        {
            try
            {
                var itemList = _context.DoctorEducationInfo
                    .Where(x => x.UserId == request.UserId)
                    .ToList();
                if (itemList.Count > 0)
                {
                    foreach (var item in itemList)
                    {
                        _context.DoctorEducationInfo.Remove(item);
                    }
                    _context.SaveChanges();
                }
                if (request.ArrDoctorEducationalViewModel.Length > 0)
                {
                    foreach (var item in request.ArrDoctorEducationalViewModel)
                    {
                        DoctorEducationInfo doctorEducationmodel = new DoctorEducationInfo();
                        doctorEducationmodel.UserId = request.UserId;
                        doctorEducationmodel.DegreeId = item.DegreeId;
                        doctorEducationmodel.InstituteName = item.InstituteName;
                        _context.DoctorEducationInfo.Add(doctorEducationmodel);
                        _context.SaveChanges();
                    }
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

        public async Task<DoctorBasicInfoViewModel> GetDoctorPersonalInfo(string userId)
        {
            return await (
                from u in _context.ApplicationUser
                join gm in _context.GenderMaster on u.GenderId equals gm.Id into gmGroup
                from gm in gmGroup.DefaultIfEmpty()
                join ua in _context.UserAddress on u.Id equals ua.UserId into uaGroup
                from ua in uaGroup.DefaultIfEmpty()
                join cm in _context.CountryMaster on ua.CountryId equals cm.Id into cmGroup
                from cm in cmGroup.DefaultIfEmpty()
                join sm in _context.StateMaster on ua.StateId equals sm.Id into smGroup
                from sm in smGroup.DefaultIfEmpty()
                join nm in _context.NationalityMaster on ua.NationalityId equals nm.Id into nmGroup
                from nm in nmGroup.DefaultIfEmpty()
                where u.Id == userId
                select new DoctorBasicInfoViewModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    GenderId = gm.Id,
                    Gender = gm.Name,
                    DateOfBirth =
                        (u.DateOfBirth != null)
                            ? u.DateOfBirth.Value.ToString(GlobalVariables.DefaultDateFormat)
                            : "",
                    About = u.About,
                    ProfilePic =
                        (!string.IsNullOrEmpty(u.ProfilePic)) ? u.ProfilePic : string.Empty,
                    PhoneNumber = u.PhoneNumber,
                    IsPhoneNoVerified = u.PhoneNumberConfirmed,
                    DialCode = u.DialCode,
                    CurrentAddress = ua.CurrentAddress,
                    CountryId = cm.Id,
                    Country = (!string.IsNullOrEmpty(cm.Name)) ? cm.Name : string.Empty,
                    StateId = sm.Id,
                    State = (!string.IsNullOrEmpty(sm.Name)) ? sm.Name : string.Empty,
                    City = (!string.IsNullOrEmpty(ua.City)) ? ua.City : string.Empty,
                    NationalityId = nm.Id,
                    Nationality = (!string.IsNullOrEmpty(nm.Name)) ? nm.Name : string.Empty,
                    DoctorLangauges = (
                        from dl in _context.DoctorLanguageInfo
                        join lm in _context.LanguageMaster on dl.LanguageId equals lm.Id
                        where dl.UserId == userId
                        select new LanguageInfoViewModel
                        {
                            Id = dl.LanguageId,
                            Name = (!string.IsNullOrEmpty(lm.Name)) ? lm.Name : string.Empty
                            //Name = lm.Name,
                        }
                    ).ToList()
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<DoctorWorkInfoViewModel> GetDoctorWorkInfo(string userId)
        {
            return await (
                from u in _context.ApplicationUser
                join w in _context.WalletBillingInfo on u.Id equals w.UserId into wbGroup
                from w in wbGroup.DefaultIfEmpty()
                where u.Id == userId
                select new DoctorWorkInfoViewModel
                {
                    Experience = u.Experience,
                    RegNo = u.RegNo,
                    AppointmentFees = w.Fee,
                    DoctorSpecialities = (
                        from ds in _context.DoctorSpecialityInfo
                        join sm in _context.SpecialityMaster on ds.SpecialityId equals sm.Id
                        where ds.UserId == userId
                        select new SpecialityInfoViewModel
                        {
                            Id = ds.SpecialityId,
                            Name = (!string.IsNullOrEmpty(sm.Name)) ? sm.Name : string.Empty
                        }
                    ).ToList(),
                    DoctorEducation = (
                        from de in _context.DoctorEducationInfo
                        join dm in _context.DegreeMaster on de.DegreeId equals dm.Id
                        where de.UserId == userId
                        select new EducationInfoViewModel
                        {
                            Id = de.DegreeId,
                            InstituteName =
                                (!string.IsNullOrEmpty(de.InstituteName))
                                    ? de.InstituteName
                                    : string.Empty,
                            DegreeName = (!string.IsNullOrEmpty(dm.Name)) ? dm.Name : string.Empty
                        }
                    ).ToList()
                }
            ).FirstOrDefaultAsync();
        }

        public bool AddDoctorBankInfo(DoctorBankInfo model)
        {
            try
            {
                var itemList = _context.DoctorBankInfo
                    .Where(x => x.UserId == model.UserId)
                    .ToList();
                if (itemList.Count > 0)
                {
                    foreach (var item in itemList)
                    {
                        _context.DoctorBankInfo.Remove(item);
                    }
                    _context.SaveChanges();
                }
                _context.DoctorBankInfo.Add(model);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DoctorBankInfo GetDoctorBankInfo(string UserId)
        {
            var _BankInfo = _context.DoctorBankInfo
                .Where(x => x.UserId == UserId)
                .Select(
                    x =>
                        new DoctorBankInfo
                        {
                            Id = x.Id,
                            BankName = x.BankName,
                            AccountNumber = x.AccountNumber,
                            RouteNo = x.RouteNo,
                            BranchCode = x.BranchCode,
                            PostCode = x.PostCode,
                            Address = x.Address
                        }
                )
                .FirstOrDefault();

            return _BankInfo;
        }

        public FilterationResponseModel<AdminDoctorsListViewModel> GetAdminDoctorsList(
            FilterationListViewModel model
        )
        {
            if (_context != null)
            {
                var source = (
                    from u in _context.ApplicationUser.OrderByDescending(a=>a.CreatedDate)
                    join ur in _context.UserRoles on u.Id equals ur.UserId
                    join r in _context.Roles on ur.RoleId equals r.Id
                    join g in _context.GenderMaster on u.GenderId equals g.Id
                    where
                        r.Name.ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower()
                        && (u.IsDeleted == false) 
                    select new AdminDoctorsListViewModel
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
                        RegNo = u.RegNo,
                        Gender = g.Name,
                        ApplicationStatus = u.ApplicationStatus,
                        ApplicationStatusDetails = GlobalVariables.GetDoctorApplicationStatus(
                            u.ApplicationStatus
                        )
                    }
                ).AsQueryable();

                // searching
                if (!string.IsNullOrWhiteSpace(model.searchQuery))
                {
                    var search = model.searchQuery.ToLower();
                    source = source.Where(
                        x =>
                            x.FirstName.ToLower().Contains(search)
                            || x.LastName.ToLower().Contains(search)
                            || x.Email.ToLower().Contains(search)
                            || x.PhoneNumber.ToLower().Contains(search)
                            || x.Gender.ToLower().Contains(search)
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
                FilterationResponseModel<AdminDoctorsListViewModel> obj =
                    new FilterationResponseModel<AdminDoctorsListViewModel>();
                obj.totalCount = TotalCount;
                obj.pageSize = PageSize;
                obj.currentPage = CurrentPage;
                obj.totalPages = TotalPages;
                obj.previousPage = previousPage;
                obj.nextPage = nextPage;
                obj.searchQuery = string.IsNullOrEmpty(model.searchQuery)
                    ? "no parameter passed"
                    : model.searchQuery;
                obj.dataList = items.ToList();
                return obj;
            }
            return null;
        }

        public async Task<DoctorProfileDetailsViewModel> GetDoctorDetailInfo(string userId)
        {
            try
            {
                DoctorProfileDetailsViewModel objDoctorProfileDetails =
                    new DoctorProfileDetailsViewModel();

                var doctorProfileInfo = (
                    from u in _context.ApplicationUser
                    where u.Id == userId
                    select new DoctorProfileInfoViewModel
                    {
                        Id = u.Id,
                        Email = u.Email,
                        IsEmailVerified = u.EmailConfirmed,
                        IsPhoneNoVerified = u.PhoneNumberConfirmed,
                        ApplicationStatus = u.ApplicationStatus,
                        ApplicationStatusDetails = GlobalVariables.GetDoctorApplicationStatus(
                            u.ApplicationStatus
                        ),
                        ProfilePic = u.ProfilePic,
                    }
                ).FirstOrDefault();

                objDoctorProfileDetails.DoctorProfileInfo = doctorProfileInfo;
                objDoctorProfileDetails.DoctorBasicInfo = await GetDoctorPersonalInfo(userId);
                objDoctorProfileDetails.DoctorWorkInfo = await GetDoctorWorkInfo(userId);
                objDoctorProfileDetails.DoctorBankInfo = GetDoctorBankInfo(userId);
                return objDoctorProfileDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateDoctorApplicationStatus(UpdateDoctorApplicationStatus model)
        {
            try
            {
                var user = _context.ApplicationUser
                    .Where(x => x.Id == model.UserId)
                    .FirstOrDefault();
                if (user != null)
                {
                    user.ApplicationStatus = model.ApplicationStatus;
                    _context.Update(user);
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

        public async Task<DoctorDashboardInfoViewModel> GetDoctorDashboardInfo(string userId)
        {
            return await _context.ApplicationUser
                .Where(x => x.Id == userId)
                .Select(
                    x =>
                        new DoctorDashboardInfoViewModel
                        {
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            ProfilePic = !string.IsNullOrEmpty(x.ProfilePic)
                                ? x.ProfilePic
                                : string.Empty,
                            DoctorSpecialities = (
                                from ds in _context.DoctorSpecialityInfo
                                join sm in _context.SpecialityMaster on ds.SpecialityId equals sm.Id
                                where ds.UserId == userId
                                select new SpecialityInfoViewModel
                                {
                                    Id = ds.SpecialityId,
                                    Name = sm.Name,
                                }
                            ).ToList(),
                            TotalEarnings = 0,
                            PercentageProfileComplete =
                                _authService.GetDoctorProfileCompletePersentById(userId),
                            TotalUpcomingAppointments =
                                _appointmentService.TotalUpcomingAppointments(userId),
                            TotalPendingAppointments = _appointmentService.TotalPendingAppointments(
                                userId
                            ),
                            TotalPastAppointments = _appointmentService.TotalPastAppointments(
                                userId
                            )
                        }
                )
                .FirstOrDefaultAsync();
        }

        public FilterationResponseModel<GetTopDoctorsViewModel> GetTopDoctors(
            FilterationListViewModel model
        )
        {
            if (_context != null)
            {
                //var _scheduleList = _context.DoctorSchedule.Where(x => x.IsDeleted == false).Select(x => x.UserId).ToList();
                var source = (
                    from u in _context.ApplicationUser
                    join ur in _context.UserRoles on u.Id equals ur.UserId
                    join r in _context.Roles on ur.RoleId equals r.Id
                    //   join w in _context.WalletBillingInfo
                    //   on u.Id equals w.UserId
                    where
                        r.Name.ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower()
                        && (u.IsDeleted == false)
                    //&& u.ApplicationStatus == (int)GlobalVariables.DoctorApplicationStatus.Accepted
                    //&& _scheduleList.Contains(u.Id)
                    select new GetTopDoctorsViewModel
                    {
                        Id = u.Id,
                        Name = u.FirstName + " " + u.LastName,
                        ProfilePic =
                            (!string.IsNullOrEmpty(u.ProfilePic)) ? u.ProfilePic : string.Empty,
                        Rating = Convert.ToDecimal(
                            (
                                from r in _context.UserRating
                                where r.DoctorId == u.Id && r.Rating > 0
                                select (int?)r.Rating
                            ).Average() ?? 0.0
                        ),
                        //AppointmentFees = w.Fee,
                        DoctorSpecialities = (
                            from ds in _context.DoctorSpecialityInfo
                            join sm in _context.SpecialityMaster on ds.SpecialityId equals sm.Id
                            where ds.UserId == u.Id
                            select new SpecialityInfoViewModel
                            {
                                Id = ds.SpecialityId,
                                Name = sm.Name,
                            }
                        ).ToList()
                    }
                ).AsQueryable();

                // searching
                if (!string.IsNullOrWhiteSpace(model.searchQuery))
                {
                    var search = model.searchQuery.ToLower();
                    source = source.Where(
                        x =>
                            x.Name.ToLower().Contains(search)
                            || x.Rating.ToString().Contains(search)
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
                var items = source?.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                // if CurrentPage is greater than 1 means it has previousPage
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                // if TotalPages is greater than CurrentPage means it has nextPage
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                // Returing List of Customers Collections
                FilterationResponseModel<GetTopDoctorsViewModel> obj =
                    new FilterationResponseModel<GetTopDoctorsViewModel>();
                obj.totalCount = TotalCount;
                obj.pageSize = PageSize;
                obj.currentPage = CurrentPage;
                obj.totalPages = TotalPages;
                obj.previousPage = previousPage;
                obj.nextPage = nextPage;
                obj.searchQuery = string.IsNullOrEmpty(model.searchQuery)
                    ? "no parameter passed"
                    : model.searchQuery;
                obj.dataList = items.ToList();
                return obj;
            }
            return null;
        }

        public FilterationResponseModel<GetTopDoctorsViewModel> GetAllDoctors(
            FilterationListViewModel model
        )
        {
            if (_context != null)
            {
                var _scheduleList = _context.DoctorSchedule
                    .Where(x => x.IsDeleted == false)
                    .Select(x => x.UserId)
                    .ToList();

                var source = (
                    from u in _context.ApplicationUser
                    join ur in _context.UserRoles on u.Id equals ur.UserId
                    join r in _context.Roles on ur.RoleId equals r.Id
                    join w in _context.WalletBillingInfo on u.Id equals w.UserId
                    where
                        r.Name.ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower()
                        && (u.IsDeleted == false)
                        // && u.ApplicationStatus == (int)GlobalVariables.DoctorApplicationStatus.Accepted
                        && _scheduleList.Contains(u.Id)
                    select new GetTopDoctorsViewModel
                    {
                        Id = u.Id,
                        Name = u.FirstName + " " + u.LastName,
                        ProfilePic = u.ProfilePic,
                        Rating = Convert.ToDecimal(
                            (
                                from r in _context.UserRating
                                where r.DoctorId == u.Id && r.Rating > 0
                                select (int?)r.Rating
                            ).Average() ?? 0.0
                        ),
                        AppointmentFees = w.Fee,
                        DoctorSpecialities = (
                            from ds in _context.DoctorSpecialityInfo
                            join sm in _context.SpecialityMaster on ds.SpecialityId equals sm.Id
                            where ds.UserId == u.Id
                            select new SpecialityInfoViewModel
                            {
                                Id = ds.SpecialityId,
                                Name = sm.Name,
                            }
                        ).ToList()
                    }
                ).ToList();

                // searching
                if (!string.IsNullOrWhiteSpace(model.searchQuery))
                {
                    var search = model.searchQuery.ToLower();
                    source = source
                        .Where(
                            x =>
                                x.Name.ToLower().Contains(search)
                                || x.Rating.ToString().Contains(search)
                        )
                        .ToList();
                }

                if (!string.IsNullOrEmpty(model.filterBy))
                {
                    if (CommonFunctions.checkIsNumeric(model.filterBy))
                    {
                        var filterBy = model.filterBy;
                        source = source
                            .Where(
                                m =>
                                    m.DoctorSpecialities.Any(y => y.Id == Convert.ToInt32(filterBy))
                            )
                            .ToList();
                    }
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
                FilterationResponseModel<GetTopDoctorsViewModel> obj =
                    new FilterationResponseModel<GetTopDoctorsViewModel>();
                obj.totalCount = TotalCount;
                obj.pageSize = PageSize;
                obj.currentPage = CurrentPage;
                obj.totalPages = TotalPages;
                obj.previousPage = previousPage;
                obj.nextPage = nextPage;
                obj.searchQuery = string.IsNullOrEmpty(model.searchQuery)
                    ? "no parameter passed"
                    : model.searchQuery;
                obj.dataList = items.ToList();
                return obj;
            }
            return null;
        }

        public async Task<GetDoctorDetailsViewModel> GetDoctorDescription(string doctorId)
        {
            decimal nRating = 0;
            var rating = _context.UserRating.Where(a => a.DoctorId == doctorId && a.Rating > 0).FirstOrDefault();
            if (rating != null)
            {
                 nRating = Convert.ToDecimal(
                    (
                        from r in _context.UserRating
                        where r.DoctorId == doctorId && r.Rating > 0
                        select r.Rating
                    ).Average()
                );
            }
            int nReviewsCount = 0;
            var reviewsCount = _context.UserRating.Where(a => a.DoctorId == doctorId && a.Rating > 0).FirstOrDefault();
            if (reviewsCount != null)
            {
                 nReviewsCount = Convert.ToInt32((from r in _context.UserRating
                    where r.DoctorId == doctorId && r.Comments != " "
                    select r.Comments).Count());
            }

            var _doctorDetails = await (
                from x in _context.ApplicationUser
                //join w in _context.WalletBillingInfo
                //on x.Id equals w.UserId
                where x.Id == doctorId && x.IsDeleted == false
                //&& x.ApplicationStatus == (int)GlobalVariables.DoctorApplicationStatus.Accepted
                select new GetDoctorDetailsViewModel
                {
                    Name = x.FirstName + " " + x.LastName,
                    ProfilePic = x.ProfilePic,
                    Rating = nRating,
                    ReviewsCount = nReviewsCount,
                    About = (!string.IsNullOrEmpty(x.About)) ? x.About : string.Empty,
                    //AppointmentFees = w.Fee,
                    DoctorSpecialities = (
                        from ds in _context.DoctorSpecialityInfo
                        join sm in _context.SpecialityMaster on ds.SpecialityId equals sm.Id
                        where ds.UserId == doctorId
                        select new SpecialityInfoViewModel { Id = ds.SpecialityId, Name = sm.Name, }
                    ).ToList(),
                    DoctorEducation = (
                        from de in _context.DoctorEducationInfo
                        join dm in _context.DegreeMaster on de.DegreeId equals dm.Id
                        where de.UserId == doctorId
                        select new EducationInfoViewModel
                        {
                            Id = de.DegreeId,
                            InstituteName = de.InstituteName,
                            DegreeName = dm.Name
                        }
                    ).ToList(),
                    DoctorLangauges = (
                        from dl in _context.DoctorLanguageInfo
                        join lm in _context.LanguageMaster on dl.LanguageId equals lm.Id
                        where dl.UserId == doctorId
                        select new LanguageInfoViewModel { Id = dl.LanguageId, Name = lm.Name, }
                    ).ToList()
                }
            ).FirstOrDefaultAsync();

            return _doctorDetails;
        }

        public decimal GetAverageRating(string doctorId)
        {
            if (_context != null)
            {
                var data = _context.UserRating.Where(x => x.Rating > 0 && x.DoctorId == doctorId);
                decimal sum = Convert.ToDecimal(data.Sum(x => x.Rating));
                decimal count = Convert.ToDecimal(data.Count());
                decimal average = Convert.ToDecimal(sum / count);
                return average;
            }
            else
            {
                return 0;
            }
        }
    }
}
