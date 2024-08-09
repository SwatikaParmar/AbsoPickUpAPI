using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.Repositories;
using AnfasAPI.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AnfasAPI.Common.GlobalVariables;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private IContentService _contentService;
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private IUploadFiles _uploadFiles;

        public AdminController(
            ApplicationDbContext context,
            IDoctorService doctorService,
            IEmailSender emailSender,
            IContentService contentService,
            UserManager<ApplicationUser> userManager,
            IPatientService patientService,
            IAuthService authService,
            IMapper mapper,
            IUploadFiles uploadFiles
        )
        {
            _doctorService = doctorService;
            _userManager = userManager;
            _patientService = patientService;
            _authService = authService;
            _emailSender = emailSender;
            _contentService = contentService;
            _context = context;
            _mapper = mapper;
            _uploadFiles = uploadFiles;

        }

        #region getDashboardData
        [HttpGet]
        [Authorize]
        [Route("getDashboardData")]
        public IActionResult getDashboardData()
        {
            try
            {
                var source = (
                    from u in _context.ApplicationUser
                    join ur in _context.UserRoles on u.Id equals ur.UserId
                    join r in _context.Roles on ur.RoleId equals r.Id
                    //join w in _context.WalletBillingInfo
                    //on u.Id equals w.UserId
                    where
                        r.Name.ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower()
                        && (u.IsDeleted == false)
                    select new GetTopDoctorsViewModel { Id = u.Id }
                ).ToList();

                var getBlogList = _context.BlogDetails
                    .Where(i => (i.IsAdminApproved == false) && (i.BlogStatus == (int)GlobalVariables.BlogStatus.Pending))
                    .OrderByDescending(i => i.CreatedDate)
                    .Take(10)
                    .Select(
                        i =>
                            new BlogListViewModel
                            {
                                blogId = i.Id,
                                userId = i.UserId,
                                title = i.Title,
                                description = i.Description,
                                blogImagePath = i.BlogImagePath,
                                isAdminApproved = i.IsAdminApproved,
                                blogStatus = i.BlogStatus
                            }
                    )
                    .ToList();

                var getAppointmentList = _context.Appointments
                    .Join(
                        _context.DoctorTimeSlots,
                        ap => ap.TimeSlotId,
                        dts => dts.Id,
                        (ap, dts) => new { ap, dts }
                    )
                    .Join(
                        _context.ApplicationUser,
                        app => app.ap.DoctorId,
                        au => au.Id,
                        (app, au) => new { app, au }
                    )
                    .Where(x => x.app.ap.IsDeleted == false)
                    .Where(x => x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Pending)
                    .OrderByDescending(x => x.app.ap.CreatedDate)
                    .Select(
                        x =>
                            new AppointmentListViewModel
                            {
                                appointmentId = x.app.ap.Id,
                                appointmentDate = x.app.dts.Date.ToString(),
                                appointmentStartTime = x.app.dts.SlotFrom.ToString(),
                                appointmentEndTime = x.app.dts.SlotTo.ToString(),
                                appointmentstatus = (int)GlobalVariables.AppointmentStatus.Pending,
                                appointmentFee = x.app.ap.Fee,
                                doctorName = x.au.FirstName + " " + x.au.LastName,
                                doctorEmail = x.au.Email,
                                doctorPhone = x.au.PhoneNumber,
                                // firstName = e.BlogStatus,
                                // lastName = e.BlogStatus,
                                // patientPhoneNumber = e.BlogStatus,
                            }
                    )
                    .ToList();

                int scheduledAppointmentCount = 0;
                var scheduledAppointmentList = new List<AppointmentListViewModel>();
                foreach (var ap in getAppointmentList)
                {
                    if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Pending)
                    {
                        if (Convert.ToDateTime(ap.appointmentDate) > DateTime.UtcNow)
                        {
                            scheduledAppointmentCount = scheduledAppointmentCount + 1;
                            ap.appointmentstatusDisplay = "Pending";
                            ap.appointmentStartTime = ap.appointmentStartTime.Replace(
                                ":00.0000000",
                                ""
                            );
                            ap.appointmentEndTime = ap.appointmentEndTime.Replace(":00.0000000", "");
                            scheduledAppointmentList.Add(ap);
                        }
                    }
                }


                // Get Total Appointments
                var getTotalAppointments = _context.Appointments.ToList().Count();

                var dashboardModel = new DashboardViewModel();
                dashboardModel.totalDoctors = source.Count();
                dashboardModel.totalScheduledAppointments = scheduledAppointmentCount;
                dashboardModel.totalAppointments = getTotalAppointments;
                dashboardModel.appointmentList = scheduledAppointmentList.Take(10).ToList();
                dashboardModel.blogList = getBlogList;

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        data = dashboardModel,
                        message = "Dashboard data" + ResponseMessages.msgAdditionSuccess,
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region getPatientRequestForReport
        [HttpGet]
        [Authorize]
        [Route("getPatientRequestForReport")]
        public async Task<IActionResult> GetPatientRequestForReport([FromQuery] FilterationListViewModel model)
        {
            //ApplicationUser _appuser = new ApplicationUser();
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }

                var requestList = await _context.RequestReport.OrderByDescending(a => a.ModifyDate).ToListAsync();
                if (requestList.Count > 0)
                {
                    // var patientReportRequest = _mapper.Map<PatientReportRequestViewModel>(user);
                    var responseRequestList = new List<PatientReportRequestViewModel>();

                    foreach (var item in requestList)
                    {
                        var user = await _context.ApplicationUser.Where(a => a.Id == item.PatientId).FirstOrDefaultAsync();
                        var patientReportRequest = new PatientReportRequestViewModel();

                        var totalTestCount = 0;
                        var getBookedLabServices = _context.BookLabService.Where(a => a.PatientId == item.PatientId).ToList();
                        if (getBookedLabServices.Count > 0)
                        {
                            totalTestCount = totalTestCount + getBookedLabServices.Count;
                        }
                        var getBookedRadiologyServices = _context.BookRadiologyService.Where(a => a.PatientId == item.PatientId).ToList();
                        if (getBookedRadiologyServices.Count > 0)
                        {
                            totalTestCount = totalTestCount + getBookedRadiologyServices.Count;
                        }
                        var getBookedDialysisServices = _context.BookDialysisService.Where(a => a.PatientId == item.PatientId).ToList();
                        if (getBookedDialysisServices.Count > 0)
                        {
                            totalTestCount = totalTestCount + getBookedDialysisServices.Count;
                        }
                        var getBookedRehabilationServices = _context.BookRehabilationService.Where(a => a.PatientId == item.PatientId).ToList();
                        if (getBookedRehabilationServices.Count > 0)
                        {
                            totalTestCount = totalTestCount + getBookedRehabilationServices.Count;
                        }

                        patientReportRequest.requestReportId = item.RequestReportId;
                        patientReportRequest.patientId = item.PatientId;
                        patientReportRequest.patientEmail = item.PatientEmail;
                        patientReportRequest.status = ((ReportStatus)item.Status).ToString();
                        patientReportRequest.requestDate = item.CreateDate.ToString(@"yyyy-MM-dd");
                        patientReportRequest.firstName = user.FirstName;
                        patientReportRequest.LastName = user.LastName;
                        patientReportRequest.totalTestCount = totalTestCount;
                        patientReportRequest.patientAge = GetAge(Convert.ToDateTime(user.DateOfBirth));

                        responseRequestList.Add(patientReportRequest);
                    }
                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        responseRequestList = responseRequestList.Where(x => (x.patientEmail.ToLower().Contains(search))
                        || (x.status.ToLower().Contains(search))
                        ).ToList();
                    }

                    // Get's No of Rows Count   
                    int count = responseRequestList.Count();

                    // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                    int CurrentPage = model.pageNumber;

                    // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                    int PageSize = model.pageSize;

                    // Display TotalCount to Records to User  
                    int TotalCount = count;

                    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = responseRequestList.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    // if CurrentPage is greater than 1 means it has previousPage  
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";

                    // if TotalPages is greater than CurrentPage means it has nextPage  
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    // Returing List of Customers Collections  
                    FilterationResponseModel<PatientReportRequestViewModel> obj = new FilterationResponseModel<PatientReportRequestViewModel>();
                    obj.totalCount = TotalCount;
                    obj.pageSize = PageSize;
                    obj.currentPage = CurrentPage;
                    obj.totalPages = TotalPages;
                    obj.previousPage = previousPage;
                    obj.nextPage = nextPage;
                    obj.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                    obj.dataList = items.ToList();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Patient request shown successfully.",
                            code = StatusCodes.Status200OK,
                            data = obj
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgUserNotFound,
                            code = StatusCodes.Status200OK
                        }
                    );
                }


            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }

        #endregion

        #region getPatientBookedServiceList
        [HttpGet]
        [Authorize]
        [Route("getPatientBookedServiceList")]
        public async Task<IActionResult> GetPatientBookedServiceList([FromQuery] string patientId, string searchQuery, string filterBy)
        {
            //ApplicationUser _appuser = new ApplicationUser();
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }

                var user = await _context.ApplicationUser.Where(a => a.Id == patientId).FirstOrDefaultAsync();
                var age = GetAge(Convert.ToDateTime(user.DateOfBirth));
                var requestReport = await _context.RequestReport.Where(a => a.PatientId == patientId).FirstOrDefaultAsync();


                List<ResponseBookedServiceViewModel> responseModel =
                    new List<ResponseBookedServiceViewModel>();

                // Get Lab service Detail
                var getBookedLabServices = _context.BookLabService.Where(a => a.PatientId == patientId).OrderByDescending(a=>a.CreateDate).ToList();

                if (getBookedLabServices.Count > 0)
                {
                    foreach (var i in getBookedLabServices)
                    {
                        var bookedLabService = new ResponseBookedServiceViewModel();
                        bookedLabService.patientId = i.PatientId;
                        bookedLabService.bookingId = i.BookLabServiceId;
                        bookedLabService.bookingDate = i.FromDate;
                        bookedLabService.BookingTime = i.FromTime;
                        bookedLabService.reason = i.Reason;
                        bookedLabService.serviceType = "Lab Service";
                        bookedLabService.servicePlanId = i.LabServicesPlanId;
                        bookedLabService.serviceId = 1;
                        bookedLabService.firstName = user.FirstName;
                        bookedLabService.lastName = user.LastName;
                        bookedLabService.patientAge = age;
                        bookedLabService.requestReportId = requestReport.RequestReportId;
                        var reportstatus = await _context.ReportDetail.Where(a => (a.BookServiceId == i.BookLabServiceId) && (a.ServiceId == 1)).FirstOrDefaultAsync();
                        if (reportstatus != null)
                        {
                            bookedLabService.reportDetailId = reportstatus.ReportDetailId;
                            bookedLabService.status = ((ReportStatus)reportstatus.Status).ToString();
                            bookedLabService.reportLink = reportstatus.ReportPath != null ? ImageUrl + reportstatus.ReportPath : "N/A";
                        }
                        else
                        {
                            bookedLabService.status = ReportStatus.Pending.ToString();
                            bookedLabService.reportLink = "N/A";
                        }
                        responseModel.Add(bookedLabService);
                    }
                }
                // Get Radiology service Detail
                var getBookedRadiologyServices = _context.BookRadiologyService.Where(a => a.PatientId == patientId).ToList();

                if (getBookedRadiologyServices.Count > 0)
                {
                    foreach (var i in getBookedRadiologyServices)
                    {
                        var bookedRadiologyService = new ResponseBookedServiceViewModel();
                        bookedRadiologyService.patientId = i.PatientId;
                        bookedRadiologyService.bookingDate = i.BookingDate;
                        bookedRadiologyService.BookingTime = i.BookingTime;
                        bookedRadiologyService.reason = i.Reason;
                        bookedRadiologyService.bookingId = i.BookRadiologyServiceId;
                        bookedRadiologyService.serviceType = "Radiology Service";
                        bookedRadiologyService.servicePlanId = i.RadiologyServicePlanId;
                        bookedRadiologyService.serviceId = 2;
                        bookedRadiologyService.firstName = user.FirstName;
                        bookedRadiologyService.lastName = user.LastName;
                        bookedRadiologyService.patientAge = age;
                        bookedRadiologyService.requestReportId = requestReport.RequestReportId;
                        var reportstatus = await _context.ReportDetail.Where(a => (a.BookServiceId == i.BookRadiologyServiceId) && (a.ServiceId == 2)).FirstOrDefaultAsync();
                        if (reportstatus != null)
                        {
                            bookedRadiologyService.reportDetailId = reportstatus.ReportDetailId;
                            bookedRadiologyService.status = ((ReportStatus)reportstatus.Status).ToString();
                            bookedRadiologyService.reportLink = reportstatus.ReportPath != null ? ImageUrl + reportstatus.ReportPath : "N/A";
                        }
                        else
                        {
                            bookedRadiologyService.status = ReportStatus.Pending.ToString();
                            bookedRadiologyService.reportLink = "N/A";
                        }

                        responseModel.Add(bookedRadiologyService);
                    }
                }
                // Get Dialysis service Detail
                var getBookedDialysisServices = _context.BookDialysisService.Where(a => a.PatientId == patientId).ToList();

                if (getBookedDialysisServices.Count > 0)
                {
                    foreach (var i in getBookedDialysisServices)
                    {
                        var bookedDialysisService = new ResponseBookedServiceViewModel();
                        bookedDialysisService.patientId = i.PatientId;
                        bookedDialysisService.bookingDate = i.BookingDate;
                        bookedDialysisService.BookingTime = i.BookingTime;
                        bookedDialysisService.reason = i.Reason;
                        bookedDialysisService.serviceType = "Dialysis Service";
                        bookedDialysisService.servicePlanId = i.DialysisServicePlanId;
                        bookedDialysisService.bookingId = i.BookDialysisServiceId;
                        bookedDialysisService.serviceId = 3;
                        bookedDialysisService.firstName = user.FirstName;
                        bookedDialysisService.lastName = user.LastName;
                        bookedDialysisService.patientAge = age;
                        bookedDialysisService.requestReportId = requestReport.RequestReportId;

                        var reportstatus = await _context.ReportDetail.Where(a => (a.BookServiceId == i.BookDialysisServiceId) && (a.ServiceId == 3)).FirstOrDefaultAsync();
                        if (reportstatus != null)
                        {
                            bookedDialysisService.reportDetailId = reportstatus.ReportDetailId;
                            bookedDialysisService.status = ((ReportStatus)reportstatus.Status).ToString();
                            bookedDialysisService.reportLink = reportstatus.ReportPath != null ? ImageUrl + reportstatus.ReportPath : "N/A";
                        }
                        else
                        {
                            bookedDialysisService.status = ReportStatus.Pending.ToString();
                            bookedDialysisService.reportLink = "N/A";
                        }

                        responseModel.Add(bookedDialysisService);
                    }
                }
                // Get Radiology service Detail
                var getBookedRehabilationServices = _context.BookRehabilationService.Where(a => a.PatientId == patientId).ToList();

                if (getBookedRehabilationServices.Count > 0)
                {
                    foreach (var i in getBookedRehabilationServices)
                    {
                        var bookedRehabilationService = new ResponseBookedServiceViewModel();
                        bookedRehabilationService.patientId = i.PatientId;
                        bookedRehabilationService.bookingDate = i.BookingDate;
                        bookedRehabilationService.BookingTime = i.BookingTime;
                        bookedRehabilationService.reason = i.Reason;
                        bookedRehabilationService.serviceType = "Rehabilation Service";
                        bookedRehabilationService.servicePlanId = i.RehabilationServicePlanId;
                        bookedRehabilationService.bookingId = i.BookRehabilationServiceId;
                        bookedRehabilationService.serviceId = 4;
                        bookedRehabilationService.firstName = user.FirstName;
                        bookedRehabilationService.lastName = user.LastName;
                        bookedRehabilationService.patientAge = age;
                        bookedRehabilationService.requestReportId = requestReport.RequestReportId;

                        var reportstatus = await _context.ReportDetail.Where(a => (a.BookServiceId == i.BookRehabilationServiceId) && (a.ServiceId == 4)).FirstOrDefaultAsync();
                        if (reportstatus != null)
                        {
                            bookedRehabilationService.reportDetailId = reportstatus.ReportDetailId;
                            bookedRehabilationService.status = ((ReportStatus)reportstatus.Status).ToString();
                            bookedRehabilationService.reportLink = reportstatus.ReportPath != null ? ImageUrl + reportstatus.ReportPath : "N/A";

                        }
                        else
                        {
                            bookedRehabilationService.status = ReportStatus.Pending.ToString();
                            bookedRehabilationService.reportLink = "N/A";
                        }

                        responseModel.Add(bookedRehabilationService);
                    }
                }

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    responseModel = responseModel.Where(x => (x.serviceType.ToLower() == searchQuery.ToLower())
                    || (x.firstName.ToLower().Contains(searchQuery.ToLower()))
                    || (x.lastName.ToLower().Contains(searchQuery.ToLower()))
                    ).ToList();
                }
                if (!string.IsNullOrEmpty(filterBy))
                {
                    responseModel = responseModel.Where(x => (x.status.ToLower() == filterBy.ToLower())).ToList();
                }


                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = "Patient booked service list shown successfully.",
                        code = StatusCodes.Status200OK,
                        data = responseModel
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }

        #endregion

        #region addPatientReport
        [HttpPost]
        [Authorize]
        [Route("addPatientReport")]
        public async Task<IActionResult> AddPatientReport(UploadBookedServiceReportViewModel model)
        {
            //ApplicationUser _appuser = new ApplicationUser();
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }

                var requestList = await _context.RequestReport.Where(a => a.RequestReportId == model.requestReportId).ToListAsync();
                if (requestList.Count > 0)
                {
                    // var patientReportRequest = _mapper.Map<PatientReportRequestViewModel>(user);
                    var reportDetail = new ReportDetail();
                    reportDetail.BookServiceId = model.bookingId;
                    reportDetail.RequestReportId = model.requestReportId;
                    var requestDetail = await _context.RequestReport.Where(a => a.RequestReportId == model.requestReportId).FirstOrDefaultAsync();
                    reportDetail.PatientId = requestDetail.PatientId;
                    reportDetail.ReportDescription = model.reportDescription;
                    reportDetail.ReportName = model.reportName;
                    reportDetail.Status = 0;

                    var reportPath = GlobalVariables.BookedServiceReportContainer;
                    reportDetail.ReportPath = reportPath;

                    _context.Update(reportDetail);
                    await _context.SaveChangesAsync();

                    var reportDetailId = reportDetail.ReportDetailId;

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Report uploaded successfully.",
                            code = StatusCodes.Status200OK,
                            data = new { reportDetailId = reportDetailId }
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = "Request not found",
                            code = StatusCodes.Status200OK
                        }
                    );
                }


            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }

        #endregion

        #region uploadPatientReportFile
        [HttpPost]
        [Authorize]
        [Route("uploadPatientReportFile")]
        public async Task<IActionResult> UploadPatientReportFile([FromForm] UploadPatientReport model)
        {
            //ApplicationUser _appuser = new ApplicationUser();
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var report = await _context.ReportDetail.Where(a => (a.BookServiceId == model.bookingId) && (a.ServiceId == model.serviceId)).FirstOrDefaultAsync();

                if (report == null)
                {
                    report = new ReportDetail();
                    report.BookServiceId = model.bookingId;
                    report.RequestReportId = model.requestReportId;
                    var request = await _context.RequestReport.Where(a => a.RequestReportId == model.requestReportId).FirstOrDefaultAsync();
                    report.PatientId = request.PatientId;
                    report.ReportDescription = model.reportDescription;
                    report.ReportName = model.reportName;
                    report.Status = 0;
                    report.ServiceId = model.serviceId;

                    await _context.AddAsync(report);
                    await _context.SaveChangesAsync();
                }

                var reportDetail = await _context.ReportDetail.Where(a => a.ReportDetailId == report.ReportDetailId).FirstOrDefaultAsync();
                if (reportDetail != null)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(model.reportFile.ContentDisposition)
                            .FileName.Trim('"');
                    filename = CommonFunctions.EnsureCorrectFilename(filename);
                    filename = CommonFunctions.RenameFileName(filename);

                    // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.reportFile,
                        GlobalVariables.BookedServiceReportContainer,
                        filename
                    );

                    var reportPath = GlobalVariables.BookedServiceReportContainer + "/" + filename;
                    reportDetail.Status = 1;
                    reportDetail.ReportPath = reportPath;

                    _context.Update(reportDetail);
                    await _context.SaveChangesAsync();

                    var requestDetail = await _context.RequestReport.Where(a => a.RequestReportId == model.requestReportId).FirstOrDefaultAsync();
                    requestDetail.Status = 1;

                    _context.Update(requestDetail);
                    await _context.SaveChangesAsync();

                    var serviceType = "";
                    var bookingDate = "";
                    if (reportDetail.ServiceId == 1)
                    {
                        serviceType = "Lab Service";
                        bookingDate = await _context.BookLabService.Where(a => a.BookLabServiceId == reportDetail.BookServiceId).Select(a => a.FromDate).FirstOrDefaultAsync();
                        // bookingDate = Convert.ToDateTime(bookingDate).ToString(@"yyyy-MM-dd");
                    }
                    else if (reportDetail.ServiceId == 2)
                    {
                        serviceType = "Radiology Service";
                        bookingDate = await _context.BookRadiologyService.Where(a => a.BookRadiologyServiceId == reportDetail.BookServiceId).Select(a => a.BookingDate).FirstOrDefaultAsync();
                        // bookingDate = Convert.ToDateTime(bookingDate).ToString(@"yyyy-MM-dd");
                    }
                    else if (reportDetail.ServiceId == 3)
                    {
                        serviceType = "Dialysis Service";
                        bookingDate = await _context.BookDialysisService.Where(a => a.BookDialysisServiceId == reportDetail.BookServiceId).Select(a => a.BookingDate).FirstOrDefaultAsync();
                        // bookingDate = Convert.ToDateTime(bookingDate).ToString(@"yyyy-MM-dd");
                    }
                    else
                    {
                        serviceType = "Rehabilation Service";
                        bookingDate = await _context.BookRehabilationService.Where(a => a.BookRehabilationServiceId == reportDetail.BookServiceId).Select(a => a.BookingDate).FirstOrDefaultAsync();
                        // bookingDate = Convert.ToDateTime(bookingDate).ToString(@"yyyy-MM-dd");
                    }

                    var user = await _context.ApplicationUser.Where(a => a.Id == requestDetail.PatientId).FirstOrDefaultAsync();

                    // System.Net.Mail.Attachment attachment;
                    // attachment = new System.Net.Mail.Attachment(filename);
                    // if (attachment != null)
                    // {
                    await _emailSender.SendEmailAsync(email: requestDetail.PatientEmail, subject: $"{serviceType} Report", message: $"Hi, {user.FirstName} <br/><br/>Here is your report detail <br/><br/>Service Type : {serviceType} <br/>Report Id : #REP{reportDetail.ReportDetailId} <br/>Report Name : {reportDetail.ReportName}<br/>Report Description : {reportDetail.ReportDescription}<br/>Booking Date : {bookingDate}<br/><br/><br/>Report Link : {ImageUrl + reportPath} <br/><br/>Thanks");
                    //     attachment.Dispose();
                    // }

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Report file uploaded successfully.",
                            code = StatusCodes.Status200OK,
                            data = new { reportLink = ImageUrl + reportPath }
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = "Not found any record",
                            code = StatusCodes.Status200OK
                        }
                    );
                }


            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }

        #endregion

        #region resendReport
        [HttpPost]
        [Authorize]
        [Route("resendReport")]
        public async Task<IActionResult> ResendReport([FromBody] ResendReportViewModel model)
        {
            //ApplicationUser _appuser = new ApplicationUser();
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }

                var report = await _context.ReportDetail.Where(a => (a.ReportDetailId == model.reportDetailId)).FirstOrDefaultAsync();
                if (report != null)
                {
                    var requestDetail = await _context.RequestReport.Where(a => a.RequestReportId == report.RequestReportId).FirstOrDefaultAsync();
                    requestDetail.Status = 1;

                    _context.Update(requestDetail);
                    await _context.SaveChangesAsync();

                    var serviceType = "";
                    var bookingDate = "";
                    if (report.ServiceId == 1)
                    {
                        serviceType = "Lab Service";
                        bookingDate = await _context.BookLabService.Where(a => a.BookLabServiceId == report.BookServiceId).Select(a => a.FromDate).FirstOrDefaultAsync();
                        // bookingDate = DateTime.ParseExact(serviceDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString(@"dd-MM-yyyy");
                    }
                    else if (report.ServiceId == 2)
                    {
                        serviceType = "Radiology Service";
                        bookingDate = await _context.BookRadiologyService.Where(a => a.BookRadiologyServiceId == report.BookServiceId).Select(a => a.BookingDate).FirstOrDefaultAsync();
                        // bookingDate = DateTime.ParseExact(serviceDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString(@"dd-MM-yyyy");
                    }
                    else if (report.ServiceId == 3)
                    {
                        serviceType = "Dialysis Service";
                        bookingDate = await _context.BookDialysisService.Where(a => a.BookDialysisServiceId == report.BookServiceId).Select(a => a.BookingDate).FirstOrDefaultAsync();
                        // bookingDate = DateTime.ParseExact(serviceDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString(@"dd-MM-yyyy");
                    }
                    else
                    {
                        serviceType = "Rehabilation Service";
                        bookingDate = await _context.BookRehabilationService.Where(a => a.BookRehabilationServiceId == report.BookServiceId).Select(a => a.BookingDate).FirstOrDefaultAsync();
                        // bookingDate = DateTime.ParseExact(serviceDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString(@"dd-MM-yyyy");
                    }

                    var user = await _context.ApplicationUser.Where(a => a.Id == requestDetail.PatientId).FirstOrDefaultAsync();

                    // System.Net.Mail.Attachment attachment;
                    // attachment = new System.Net.Mail.Attachment(filename);
                    // if (attachment != null)
                    // {
                    await _emailSender.SendEmailAsync(email: requestDetail.PatientEmail, subject: $"{serviceType} Report", message: $"Hi, {user.FirstName} <br/><br/>Here is your report detail <br/><br/>Service Type : {serviceType} <br/>Report Id : #REP{report.ReportDetailId} <br/>Report Name : {report.ReportName}<br/>Report Description : {report.ReportDescription}<br/>Booking Date : {bookingDate}<br/><br/><br/>Report Link : {ImageUrl + report.ReportPath} <br/><br/>Thanks");
                    //     attachment.Dispose();
                    // }

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Report resend successfully.",
                            code = StatusCodes.Status200OK,
                            data = new { reportLink = ImageUrl + report.ReportPath }
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            message = "Not found any record.",
                            code = StatusCodes.Status200OK,
                            data = new { }
                        }
                    );
                }



            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }

        #endregion

        public static int GetAge(DateTime birthDate)
        {
            DateTime n = DateTime.Now; // To avoid a race condition around midnight
            int age = n.Year - birthDate.Year;

            if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
                age--;

            return age;
        }
    }
}
