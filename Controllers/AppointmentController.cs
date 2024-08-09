using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IScheduleService _scheduleService;
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;
        private UserManager<ApplicationUser> _userManager;
        private readonly IWalletService _walletService;
        private readonly IContentService _contentService;
        private readonly IEmailSender _emailSenderService;
        private ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context, IAppointmentService appointmentService, IScheduleService scheduleService, INotificationService notificationService, IAuthService authService, UserManager<ApplicationUser> userManager, IWalletService walletService, IContentService contentService, IEmailSender emailSenderService)
        {
            _context = context;
            _appointmentService = appointmentService;
            _scheduleService = scheduleService;
            _notificationService = notificationService;
            _authService = authService;
            _userManager = userManager;
            _walletService = walletService;
            _contentService = contentService;
            _emailSenderService = emailSenderService;
        }

        #region "Doctor Appointment for Patients" 
        [HttpPost]
        [Route("BookAppointment")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> BookAppointment(AppointmentsViewModel _model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgParametersNotCorrect,
                        code = StatusCodes.Status200OK
                    });
                }
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }

                // int _profileCompletionStatus = _authService.GetPatientProfileCompletePersentById(currentUserId);
                // if(_profileCompletionStatus != 100)
                // {
                // return Ok(new ApiResponseModel
                // {
                //     status = false,
                //     data = new { },
                //     message =  ResponseMessages.msgNotApplicableForBookAppointment,
                //     code = StatusCodes.Status200OK
                // });
                // }

                var userWalletInfo = await _walletService.GetUserWalletInfo(_model.DoctorId);
                Appointments objAppointments = new Appointments();
                objAppointments.TimeSlotId = _model.TimeSlotId;
                objAppointments.DoctorId = _model.DoctorId;
                objAppointments.PatientId = currentUserId;
                //objAppointments.Fee = userWalletInfo.Fee;
                objAppointments.CreatedDate = DateTime.UtcNow;
                // Book Appointment
                var result = await _appointmentService.BookAppointment(objAppointments);
                if (result.Status)
                {
                    // here we set un-available slot for patients
                    await _scheduleService.UpdateTimeSlotStatus(_model.TimeSlotId, false);

                    // create notifications
                    var _patient = await _userManager.FindByIdAsync(currentUserId);
                    string patientName = _patient.FirstName + " " + _patient.LastName;

                    Notifications objNotifications = new Notifications();
                    objNotifications.FromUser = currentUserId;
                    objNotifications.ToUser = _model.DoctorId;
                    objNotifications.Type = (int)GlobalVariables.NotificationTypes.NEW_APPOINTMENT_REQUEST;
                    objNotifications.Text = NotificationMessages.GetCreateAppointmentNotificationMsg(patientName);
                    objNotifications.IsRead = false;
                    objNotifications.CreatedOn = DateTime.UtcNow;
                    objNotifications.Purpose = (int)GlobalVariables.PurposeTypes.Appointment;
                    objNotifications.PurposeId = result.AppointmentId;
                    _notificationService.CreateNotification(objNotifications);

                    // Get UserConfigurations
                    var _userConfiguration = _contentService.GetUserConfigurations(_model.DoctorId);
                    var _doctor = await _userManager.FindByIdAsync(_model.DoctorId);
                    string doctorName = _doctor.FirstName + " " + _doctor.LastName;

                    // data to be send in push notifications
                    AppointmentDetailsForNotificationViewModel appointmentDetails = new AppointmentDetailsForNotificationViewModel();
                    appointmentDetails.AppointmentId = result.AppointmentId;
                    appointmentDetails.DoctorId = _model.DoctorId;
                    appointmentDetails.PatientId = currentUserId;
                    appointmentDetails.SlotFrom = result.Date + " " + result.SlotFrom;
                    appointmentDetails.SlotTo = result.Date + " " + result.SlotTo;
                    appointmentDetails.Date = result.Date;
                    appointmentDetails.AppointmentStatus = (int)GlobalVariables.AppointmentStatus.Pending;

                    // send PushNotification
                    if (_userConfiguration.PushNotificationStatus)
                    {
                        string[] tokens = new string[] { _doctor.DeviceToken };
                        // int notificationType = (int)GlobalVariables.NotificationTypes.NEW_APPOINTMENT_REQUEST;
                        var body = PushNotificationMessages.GetCreateAppointmentPushNotificationMsg(patientName, result.SlotFrom, result.SlotTo, result.Date);
                        var data = new { my_custom_key = "New Appointment Push notification", title = PushNotificationMessages.newAppointmentRequestTitle, type = (int)GlobalVariables.NotificationTypes.NEW_APPOINTMENT_REQUEST, message = body, appointmentDetails = appointmentDetails };
                        await _notificationService.SendPushNotificationsForDoctor(tokens, PushNotificationMessages.newAppointmentRequestTitle, body, data);
                    }
                    // send Email to Doctor
                    if (_userConfiguration.EmailNotificationStatus)
                    {
                        var _drEmailmsg = EmailMessages.GetCreateAppointmentDoctorEmailNotificationMsg(doctorName, patientName, result.SlotFrom, result.SlotTo, result.Date);
                        await _emailSenderService.SendEmailAsync(email: _doctor.Email, subject: EmailMessages.GetNewAppointmentEmailSubject(patientName), message: _drEmailmsg);
                    }

                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = new { },
                        message = "Appointment request" + ResponseMessages.msgSentSuccess,
                        code = StatusCodes.Status200OK
                    });
                }
                else if (result.ResponseValue == -2)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTimeBreached,
                        code = StatusCodes.Status200OK
                    });
                }
                else if (result.ResponseValue == -3)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgIsAlreadyBooked,
                        code = StatusCodes.Status200OK
                    });
                }
                else if (result.ResponseValue == -1)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgPreBookingTimeBreached,
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = "Appointment" + ResponseMessages.msgAlreadyExists,
                        code = StatusCodes.Status200OK
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }

        #region "Get Doctor Appointments in Admin"
        [HttpGet]
        [Authorize]
        [Route("GetDoctorAppointmentsAdmin")]
        public async Task<IActionResult> GetDoctorUpcomingAppointmentsAdmin([FromQuery] string appointmentStatus, int appointmentTypeId)
        {
            try
            {
                var response = new List<AppointmentListViewModel>();
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);

                if (appointmentStatus == "past")
                {
                    if (user != null)
                    {
                        var _upcomingAppointments = _context.Appointments
                        .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                        .Join(_context.ApplicationUser, app => app.ap.DoctorId, au => au.Id, (app, au) => new { app, au })
                        .Where(x => x.app.ap.IsDeleted == false)
                        .OrderByDescending(x => x.app.ap.CreatedDate)
                        .Select(x => new AppointmentListViewModel
                        {
                            appointmentId = x.app.ap.Id,
                            appointmentDate = x.app.dts.Date.ToString(),
                            appointmentStartTime = x.app.dts.SlotFrom.ToString(),
                            appointmentEndTime = x.app.dts.SlotTo.ToString(),
                            appointmentstatus = x.app.ap.Status,
                            appointmentFee = x.app.ap.Fee,
                            doctorName = x.au.FirstName + " " + x.au.LastName,
                            doctorEmail = x.au.Email,
                            doctorPhone = x.au.PhoneNumber,
                            appointmentTypeId = 1,
                            appointmentTypeName = "Teleconsultancy"
                            // patientFirstName = e.BlogStatus,
                            // patientLastName = e.BlogStatus,
                            // patientPhoneNumber = e.BlogStatus,
                        }).ToList();


                        foreach (var item in _upcomingAppointments)
                        {
                            var appointmentDate = Convert.ToDateTime(item.appointmentDate);
                            if (appointmentDate < DateTime.UtcNow.AddDays(1) || item.appointmentstatus == 2 || item.appointmentstatus == 3 || item.appointmentstatus == 5)
                            {
                                response.Add(item);
                            }
                        }

                        // Get Home service Detail
                        var getBookHomeService = _context.BookHomeService.OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookHomeService.Count > 0)
                        {
                            foreach (var item in getBookHomeService)
                            {
                                var bookedHomeService = new AppointmentListViewModel();
                                bookedHomeService.appointmentId = item.BookHomeServiceId.ToString();
                                bookedHomeService.appointmentDate = item.FromDate;
                                bookedHomeService.appointmentStartTime = item.FromTime;
                                // bookedHomeService.appointmentEndTime = item.ToTime;
                                bookedHomeService.appointmentstatus = item.Status;
                                var doctorDetail = _context.ApplicationUser.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                                if (doctorDetail != null)
                                {
                                    bookedHomeService.doctorName = doctorDetail.FirstName + " " + doctorDetail.LastName;
                                    bookedHomeService.doctorEmail = doctorDetail.Email;
                                    bookedHomeService.doctorPhone = doctorDetail.PhoneNumber;
                                }
                                bookedHomeService.appointmentTypeId = 2;
                                bookedHomeService.appointmentTypeName = "Home";
                                var appointmentDate = DateTime.ParseExact(item.FromDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate < DateTime.UtcNow.AddDays(1) || bookedHomeService.appointmentstatus == 2 || bookedHomeService.appointmentstatus == 3 || bookedHomeService.appointmentstatus == 5)
                                {
                                    response.Add(bookedHomeService);
                                }
                            }
                        }
                        // Get Lab service Detail
                        var getBookedLabServices = _context.BookLabService.OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookedLabServices.Count > 0)
                        {
                            foreach (var item in getBookedLabServices)
                            {
                                var bookedLabService = new AppointmentListViewModel();
                                bookedLabService.appointmentId = item.BookLabServiceId.ToString();
                                bookedLabService.appointmentDate = item.FromDate;
                                bookedLabService.appointmentStartTime = item.FromTime;
                                // bookedLabService.appointmentEndTime = item.ToTime;
                                bookedLabService.appointmentstatus = item.Status;
                                bookedLabService.appointmentFee = 0;
                                var doctorDetail = _context.ApplicationUser.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                                if (doctorDetail != null)
                                {
                                    bookedLabService.doctorName = doctorDetail.FirstName + " " + doctorDetail.LastName;
                                    bookedLabService.doctorEmail = doctorDetail.Email;
                                    bookedLabService.doctorPhone = doctorDetail.PhoneNumber;
                                }
                                bookedLabService.appointmentTypeId = 3;
                                bookedLabService.appointmentTypeName = "Lab";

                                var appointmentDate = DateTime.ParseExact(item.FromDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate < DateTime.UtcNow.AddDays(1) || bookedLabService.appointmentstatus == 2 || bookedLabService.appointmentstatus == 3 || bookedLabService.appointmentstatus == 5)
                                {
                                    response.Add(bookedLabService);
                                }
                            }
                        }

                        // Get Dialysis service Detail
                        var getBookDialysisService = _context.BookDialysisService.OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookDialysisService.Count > 0)
                        {
                            foreach (var item in getBookDialysisService)
                            {
                                var bookDialysisService = new AppointmentListViewModel();
                                bookDialysisService.appointmentId = item.BookDialysisServiceId.ToString();
                                bookDialysisService.appointmentDate = item.BookingDate;
                                bookDialysisService.appointmentStartTime = item.BookingTime;
                                bookDialysisService.appointmentstatus = item.BookingStatus;
                                bookDialysisService.appointmentTypeId = 4;
                                bookDialysisService.appointmentTypeName = "Dialysis";

                                var appointmentDate = DateTime.ParseExact(item.BookingDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate < DateTime.UtcNow.AddDays(1) || bookDialysisService.appointmentstatus == 2 || bookDialysisService.appointmentstatus == 3 || bookDialysisService.appointmentstatus == 5)
                                {
                                    response.Add(bookDialysisService);
                                }
                            }
                        }

                        // Get Radiology service Detail
                        var getBookRadiologyService = _context.BookRadiologyService.OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookRadiologyService.Count > 0)
                        {
                            foreach (var item in getBookRadiologyService)
                            {
                                var bookRadiologyService = new AppointmentListViewModel();
                                bookRadiologyService.appointmentId = item.BookRadiologyServiceId.ToString();
                                bookRadiologyService.appointmentDate = item.BookingDate;
                                bookRadiologyService.appointmentStartTime = item.BookingTime;
                                bookRadiologyService.appointmentstatus = item.BookingStatus;
                                bookRadiologyService.appointmentTypeId = 5;
                                bookRadiologyService.appointmentTypeName = "Radiology";

                                var appointmentDate = DateTime.ParseExact(item.BookingDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate < DateTime.UtcNow.AddDays(1) || bookRadiologyService.appointmentstatus == 2 || bookRadiologyService.appointmentstatus == 3 || bookRadiologyService.appointmentstatus == 5)
                                {
                                    response.Add(bookRadiologyService);
                                }
                            }
                        }

                        // Get Rehabilation service Detail
                        var getBookRehabilationService = _context.BookRehabilationService.OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookRehabilationService.Count > 0)
                        {
                            foreach (var item in getBookRehabilationService)
                            {
                                var bookRehabilationService = new AppointmentListViewModel();
                                bookRehabilationService.appointmentId = item.BookRehabilationServiceId.ToString();
                                bookRehabilationService.appointmentDate = item.BookingDate;
                                bookRehabilationService.appointmentStartTime = item.BookingTime;
                                bookRehabilationService.appointmentstatus = item.BookingStatus;
                                bookRehabilationService.appointmentTypeId = 5;
                                bookRehabilationService.appointmentTypeName = "Rehabilation";

                                var appointmentDate = DateTime.ParseExact(item.BookingDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate < DateTime.UtcNow.AddDays(1) || bookRehabilationService.appointmentstatus == 2 || bookRehabilationService.appointmentstatus == 3 || bookRehabilationService.appointmentstatus == 5)
                                {
                                    response.Add(bookRehabilationService);
                                }
                            }
                        }
                        // response = response.OrderByDescending(a => DateTime.ParseExact(a.appointmentDate, "dd-MM-yyyy", new CultureInfo("en-US"))).ToList();
                        if (response.Count > 0)
                            foreach (var ap in response)
                            {
                                ap.appointmentStartTime = Convert.ToDateTime(ap.appointmentStartTime).ToString("hh:mm tt");
                                if (ap.appointmentTypeId == 1)
                                {
                                    ap.appointmentEndTime = Convert.ToDateTime(ap.appointmentEndTime).ToString("hh:mm tt");
                                    ap.appointmentDate = Convert.ToDateTime(ap.appointmentDate).ToString(@"dd-MM-yyyy");
                                }

                                if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Pending)
                                {
                                    ap.appointmentstatusDisplay = "Pending";
                                }
                                if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Confirmed)
                                {
                                    ap.appointmentstatusDisplay = "Confirmed";
                                }
                                if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Cancelled)
                                {
                                    ap.appointmentstatusDisplay = "Cancelled";
                                }
                                if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Completed)
                                {
                                    ap.appointmentstatusDisplay = "Completed";
                                }
                            }

                        if (appointmentTypeId > 0)
                        {
                            response = response.Where(a => a.appointmentTypeId == appointmentTypeId).ToList();
                        }
                        response = response.OrderByDescending(a => DateTime.ParseExact(a.appointmentDate, "dd-MM-yyyy", new CultureInfo("en-US"))).ToList();

                        if (response != null)
                        {
                            return Ok(new ApiResponseModel
                            {
                                status = true,
                                message = "Upcoming Appointments List " + ResponseMessages.msgShownSuccess,
                                data = response,
                                code = StatusCodes.Status200OK
                            });
                        }
                        else
                        {
                            return Ok(new ApiResponseModel
                            {
                                status = true,
                                message = ResponseMessages.msgNoUpcomingAppointments,
                                data = new { },
                                code = StatusCodes.Status200OK
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                    }
                }
                else
                {
                    int appointmentStatusInQuery = 0;
                    int appointmentStatusInQuery1 = 1;
                    //int appointmentStatusInQuery2 = 3;
                    if (user != null)
                    {
                        var _upcomingAppointments = _context.Appointments
                        .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                        .Join(_context.ApplicationUser, app => app.ap.DoctorId, au => au.Id, (app, au) => new { app, au })
                        .Where(x => x.app.ap.IsDeleted == false)
                        .Where(x => x.app.ap.Status == appointmentStatusInQuery || x.app.ap.Status == appointmentStatusInQuery1)
                        .OrderByDescending(x => x.app.ap.CreatedDate)
                        .Select(x => new AppointmentListViewModel
                        {
                            appointmentId = x.app.ap.Id,
                            appointmentDate = x.app.dts.Date.ToString(),
                            appointmentStartTime = x.app.dts.SlotFrom.ToString(),
                            appointmentEndTime = x.app.dts.SlotTo.ToString(),
                            appointmentstatus = x.app.ap.Status,
                            appointmentFee = x.app.ap.Fee,
                            doctorName = x.au.FirstName + " " + x.au.LastName,
                            doctorEmail = x.au.Email,
                            doctorPhone = x.au.PhoneNumber,
                            appointmentTypeId = 1,
                            appointmentTypeName = "Teleconsultancy"
                            // patientFirstName = e.BlogStatus,
                            // patientLastName = e.BlogStatus,
                            // patientPhoneNumber = e.BlogStatus,
                        }).ToList();
                        foreach (var item in _upcomingAppointments)
                        {
                            var appointmentDate = Convert.ToDateTime(item.appointmentDate);
                            if (appointmentDate > DateTime.UtcNow.AddDays(-1))
                            {
                                response.Add(item);
                            }
                        }

                        // Get Home service Detail
                        var getBookHomeService = _context.BookHomeService
                        .Where(x => (x.Status == appointmentStatusInQuery) || (x.Status == appointmentStatusInQuery))
                        .OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookHomeService.Count > 0)
                        {
                            foreach (var item in getBookHomeService)
                            {
                                var bookedHomeService = new AppointmentListViewModel();
                                bookedHomeService.appointmentId = item.BookHomeServiceId.ToString();
                                bookedHomeService.appointmentDate = item.FromDate;
                                bookedHomeService.appointmentStartTime = item.FromTime;
                                // bookedHomeService.appointmentEndTime = item.ToTime;
                                bookedHomeService.appointmentstatus = item.Status;
                                var doctorDetail = _context.ApplicationUser.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                                if (doctorDetail != null)
                                {
                                    bookedHomeService.doctorName = doctorDetail.FirstName + " " + doctorDetail.LastName;
                                    bookedHomeService.doctorEmail = doctorDetail.Email;
                                    bookedHomeService.doctorPhone = doctorDetail.PhoneNumber;
                                }
                                bookedHomeService.appointmentTypeId = 2;
                                bookedHomeService.appointmentTypeName = "Home";

                                var appointmentDate = DateTime.ParseExact(item.FromDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate > DateTime.UtcNow)
                                {
                                    response.Add(bookedHomeService);
                                }
                            }
                        }
                        // Get Lab service Detail
                        var getBookedLabServices = _context.BookLabService
                        .Where(x => (x.Status == appointmentStatusInQuery) || (x.Status == appointmentStatusInQuery))
                        .OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookedLabServices.Count > 0)
                        {
                            foreach (var item in getBookedLabServices)
                            {
                                var bookedLabService = new AppointmentListViewModel();
                                bookedLabService.appointmentId = item.BookLabServiceId.ToString();
                                bookedLabService.appointmentDate = item.FromDate;
                                bookedLabService.appointmentStartTime = item.FromTime;
                                bookedLabService.appointmentEndTime = item.ToTime;
                                bookedLabService.appointmentstatus = item.Status;
                                bookedLabService.appointmentFee = 0;
                                var doctorDetail = _context.ApplicationUser.Where(a => a.Id == item.DoctorId).FirstOrDefault();
                                if (doctorDetail != null)
                                {
                                    bookedLabService.doctorName = doctorDetail.FirstName + " " + doctorDetail.LastName;
                                    bookedLabService.doctorEmail = doctorDetail.Email;
                                    bookedLabService.doctorPhone = doctorDetail.PhoneNumber;
                                }
                                bookedLabService.appointmentTypeId = 3;
                                bookedLabService.appointmentTypeName = "Lab";

                                var appointmentDate = DateTime.ParseExact(item.FromDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate > DateTime.UtcNow)
                                {
                                    response.Add(bookedLabService);
                                }
                            }
                        }

                        // Get Dialysis service Detail
                        var getBookDialysisService = _context.BookDialysisService
                        .Where(x => (x.BookingStatus == appointmentStatusInQuery) || (x.BookingStatus == appointmentStatusInQuery))
                        .OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookDialysisService.Count > 0)
                        {
                            foreach (var item in getBookDialysisService)
                            {
                                var bookDialysisService = new AppointmentListViewModel();
                                bookDialysisService.appointmentId = item.BookDialysisServiceId.ToString();
                                bookDialysisService.appointmentDate = item.BookingDate;
                                bookDialysisService.appointmentStartTime = item.BookingTime;
                                bookDialysisService.appointmentstatus = item.BookingStatus;
                                bookDialysisService.appointmentTypeId = 4;
                                bookDialysisService.appointmentTypeName = "Dialysis";

                                var appointmentDate = DateTime.ParseExact(item.BookingDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate > DateTime.UtcNow)
                                {
                                    response.Add(bookDialysisService);
                                }
                            }
                        }

                        // Get Radiology service Detail
                        var getBookRadiologyService = _context.BookRadiologyService
                        .Where(x => (x.BookingStatus == appointmentStatusInQuery) || (x.BookingStatus == appointmentStatusInQuery))
                        .OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookRadiologyService.Count > 0)
                        {
                            foreach (var item in getBookRadiologyService)
                            {
                                var bookRadiologyService = new AppointmentListViewModel();
                                bookRadiologyService.appointmentId = item.BookRadiologyServiceId.ToString();
                                bookRadiologyService.appointmentDate = item.BookingDate;
                                bookRadiologyService.appointmentStartTime = item.BookingTime;
                                bookRadiologyService.appointmentstatus = item.BookingStatus;
                                bookRadiologyService.appointmentTypeId = 5;
                                bookRadiologyService.appointmentTypeName = "Radiology Service";

                                var appointmentDate = DateTime.ParseExact(item.BookingDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate > DateTime.UtcNow)
                                {
                                    response.Add(bookRadiologyService);
                                }
                            }
                        }

                        // Get Rehabilation service Detail
                        var getBookRehabilationService = _context.BookRehabilationService
                        .Where(x => (x.BookingStatus == appointmentStatusInQuery) || (x.BookingStatus == appointmentStatusInQuery))
                        .OrderByDescending(a => a.CreateDate).ToList();

                        if (getBookRehabilationService.Count > 0)
                        {
                            foreach (var item in getBookRehabilationService)
                            {
                                var bookRehabilationService = new AppointmentListViewModel();
                                bookRehabilationService.appointmentId = item.BookRehabilationServiceId.ToString();
                                bookRehabilationService.appointmentDate = item.BookingDate;
                                bookRehabilationService.appointmentStartTime = item.BookingTime;
                                bookRehabilationService.appointmentstatus = item.BookingStatus;
                                bookRehabilationService.appointmentTypeId = 6;
                                bookRehabilationService.appointmentTypeName = "Rehabilation";

                                var appointmentDate = DateTime.ParseExact(item.BookingDate, "dd-MM-yyyy", new CultureInfo("en-US"));
                                if (appointmentDate > DateTime.UtcNow)
                                {
                                    response.Add(bookRehabilationService);
                                }
                            }
                        }

                        foreach (var ap in response)
                        {
                            ap.appointmentStartTime = Convert.ToDateTime(ap.appointmentStartTime).ToString("hh:mm tt");
                            if (ap.appointmentTypeId == 1)
                            {
                                ap.appointmentEndTime = Convert.ToDateTime(ap.appointmentEndTime).ToString("hh:mm tt");
                                ap.appointmentDate = Convert.ToDateTime(ap.appointmentDate).ToString(@"dd-MM-yyyy");
                            }

                            if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Pending)
                            {
                                ap.appointmentstatusDisplay = "Pending";
                            }
                            if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Confirmed)
                            {
                                ap.appointmentstatusDisplay = "Confirmed";
                            }
                            if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Cancelled)
                            {
                                ap.appointmentstatusDisplay = "Cancelled";
                            }
                            if (ap.appointmentstatus == (int)GlobalVariables.AppointmentStatus.Completed)
                            {
                                ap.appointmentstatusDisplay = "Completed";
                            }
                        }
                        if (appointmentTypeId > 0)
                        {
                            response = response.Where(a => a.appointmentTypeId == appointmentTypeId).ToList();
                        }
                        response = response.OrderByDescending(a => DateTime.ParseExact(a.appointmentDate, "dd-MM-yyyy", new CultureInfo("en-US"))).ToList();

                        if (response != null)
                        {
                            return Ok(new ApiResponseModel
                            {
                                status = true,
                                message = "Upcoming Appointments List " + ResponseMessages.msgShownSuccess,
                                data = response,
                                code = StatusCodes.Status200OK
                            });
                        }
                        else
                        {
                            return Ok(new ApiResponseModel
                            {
                                status = true,
                                message = ResponseMessages.msgNoUpcomingAppointments,
                                data = new { },
                                code = StatusCodes.Status200OK
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                    }
                }


            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }
        #endregion

        [HttpPost]
        [Route("UpdateAppointmentStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateAppointmentStatus(AppointmentStatusChangeViewModel _model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgParametersNotCorrect,
                        code = StatusCodes.Status200OK
                    });
                }
                string currentUserId = CommonFunctions.getUserId(User);
                string currentUserRole = CommonFunctions.getUserRole(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                int result = await _appointmentService.UpdateAppointmentStatus(currentUserId, currentUserRole, _model);
                if (result == 1)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = new { },
                        message = "Appointment " + GlobalVariables.GetAppointmentStatusNames(_model.AppointmentStatus) + ResponseMessages.msgSuccessfully,
                        code = StatusCodes.Status200OK
                    });
                }
                else if (result == -1)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = "Appointment cannot be confirmed prior 15 minutes of Appointment time ",
                        code = StatusCodes.Status200OK
                    });
                }
                else if (result == -2)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = "Appointment cannot be cancelled prior 1 hour of appointment time ",
                        code = StatusCodes.Status200OK
                    });
                }
                else if (result == -3)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = "You can not complete appointment before start time ",
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong,
                        code = StatusCodes.Status200OK
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }
        #endregion

        [HttpGet]
        [Route("GetPatientUpcomingAppointments")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientUpcomingAppointments(int appointmentStatus)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    // here we can sent upcoming appointments based on conditions i.e [AppointmentStatus Enums] confirmed or pending
                    var _upcomingAppointments = _appointmentService.GetPatientUpcomingAppointments(user.Id, appointmentStatus);
                    if (_upcomingAppointments != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = _upcomingAppointments,
                            message = "Upcoming Appointments List" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = ResponseMessages.msgNoUpcomingAppointments,
                            data = new { },
                            code = StatusCodes.Status200OK
                        });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet]
        [Route("GetPatientPastAppointments")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientPastAppointments([FromQuery] FilterationListViewModel model)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var pastAppointments = _appointmentService.GetPatientPastAppointments(model, user.Id);
                    if (pastAppointments != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = pastAppointments,
                            message = "Past Appointments List" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = ResponseMessages.msgNoPastAppointments,
                            data = new { },
                            code = StatusCodes.Status200OK
                        });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                }

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }

        #region "Get Doctor Upcoming Appointments"
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetDoctorUpcomingAppointments")]
        // ---------Get Doctor Pending or Confirmed Appointments ------------------
        public async Task<IActionResult> GetDoctorUpcomingAppointments([FromQuery] FilterationListViewModel model, int appointmentStatus)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var _upcomingAppointments = _appointmentService.GetDoctorUpcomingAppointments(model, user.Id, appointmentStatus);
                    if (_upcomingAppointments != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = "Upcoming Appointments List " + ResponseMessages.msgShownSuccess,
                            data = _upcomingAppointments,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = ResponseMessages.msgNoUpcomingAppointments,
                            data = new { },
                            code = StatusCodes.Status200OK
                        });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }
        #endregion

        #region "Get Doctor Past Appointments"
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetDoctorPastAppointments")]
        // ----------Get Doctor Cancelled, Rejected Appointments--------

        public async Task<IActionResult> GetDoctorPastAppointments([FromQuery] FilterationListViewModel model)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var _pastAppointmentsList = _appointmentService.GetDoctorPastAppointments(model, user.Id);
                    if (_pastAppointmentsList != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = "Past Appointments List " + ResponseMessages.msgShownSuccess,
                            data = _pastAppointmentsList,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = ResponseMessages.msgNoPastAppointments,
                            data = new { },
                            code = StatusCodes.Status200OK
                        });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }
        #endregion

        //#region "Get Doctor Complete Appointments"
        //[HttpGet]
        //[Authorize(Roles = "Doctor")]
        //[Route("GetDoctorCompleteAppointments")]
        //// ----------Get Doctor Complete Appointments--------
        //public async Task<IActionResult> GetDoctorCompleteAppointments([FromQuery] FilterationListViewModel model)
        //{
        //    try
        //    {
        //        string currentUserId = CommonFunctions.getUserId(User);
        //        if (string.IsNullOrEmpty(currentUserId))
        //        {
        //            return Ok(new ApiResponseModel
        //            {
        //                status = false,
        //                data = new { },
        //                message = ResponseMessages.msgTokenExpired,
        //                code = StatusCodes.Status200OK
        //            });
        //        }
        //        var user = await _userManager.FindByIdAsync(currentUserId);
        //        if (user != null)
        //        {
        //            var _completeAppointmentsList = _appointmentService.GetDoctorCompleteAppointments(model, user.Id);
        //            if (_completeAppointmentsList != null)
        //            {
        //                return Ok(new ApiResponseModel
        //                {
        //                    status = true,
        //                    message = "Complete Appointments List " + ResponseMessages.msgShownSuccess,
        //                    data = _pastAppointmentsList,
        //                    code = StatusCodes.Status200OK
        //                });
        //            }
        //            else
        //            {
        //                return Ok(new ApiResponseModel
        //                {
        //                    status = true,
        //                    message = ResponseMessages.msgNoPastAppointments,
        //                    data = new { },
        //                    code = StatusCodes.Status200OK
        //                });
        //            }
        //        }
        //        else
        //        {
        //            return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new ApiResponseModel
        //        {
        //            status = false,
        //            data = new { },
        //            message = ResponseMessages.msgSomethingWentWrong + ex.Message,
        //            code = StatusCodes.Status500InternalServerError
        //        });
        //    }
        //}
        //#endregion


        #region "Get Appointment detail"
        [HttpGet]
        [Authorize]
        [Route("GetAppointmentDetail")]

        public async Task<IActionResult> GetAppointmentDetail([FromQuery] string appointmentId, int appointmentTypeId)
        {
            string currentUserId = CommonFunctions.getUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgTokenExpired,
                    code = StatusCodes.Status200OK
                });
            }
            var _getgAppointmentDetail = new AdminAppointmentDetailsViewModel();
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user != null)
            {
                if (appointmentTypeId == 1 || appointmentTypeId == 0)
                {
                    _getgAppointmentDetail = (from ap in _context.Appointments
                                              join dts in _context.DoctorTimeSlots
                                              on ap.TimeSlotId equals dts.Id
                                              join pt in _context.ApplicationUser
                                              on ap.PatientId equals pt.Id
                                              join doc in _context.ApplicationUser
                                              on ap.DoctorId equals doc.Id
                                              where ap.Id == appointmentId
                                              select new AdminAppointmentDetailsViewModel
                                              {
                                                  AppointmentId = ap.Id,
                                                  PatientId = ap.PatientId,
                                                  PatientFirstName = pt.FirstName,
                                                  PatientLastName = pt.LastName,
                                                  PatientEmail = pt.Email,
                                                  PatientPhoneNumber = pt.PhoneNumber,
                                                  DoctorId = ap.DoctorId,
                                                  DoctorFirstName = doc.FirstName,
                                                  DoctorLastName = doc.LastName,
                                                  DoctorPhoneNumber = doc.PhoneNumber,
                                                  DoctorEmail = doc.Email,
                                                  TimeSlotId = ap.TimeSlotId,
                                                  SlotFrom = CommonFunctions.ConvertTimeSpanToString(dts.SlotFrom),
                                                  SlotTo = CommonFunctions.ConvertTimeSpanToString(dts.SlotTo),
                                                  AppointmentDate = dts.Date.ToString(GlobalVariables.DefaultDateFormat),
                                                  AppointmentStatus = ap.Status,
                                                  Fee = ap.Fee,
                                                  appointmentTypeId = 1,
                                                  appointmentTypeName = "Teleconsultancy"
                                              }).FirstOrDefault();

                }
                // Get Home service Detail
                if (appointmentTypeId == 2)
                {
                    var getBookHomeService = _context.BookHomeService.Where(a => a.BookHomeServiceId == Convert.ToInt32(appointmentId)).FirstOrDefault();

                    if (getBookHomeService != null)
                    {
                        _getgAppointmentDetail = new AdminAppointmentDetailsViewModel();
                        _getgAppointmentDetail.AppointmentId = getBookHomeService.BookHomeServiceId.ToString();
                        _getgAppointmentDetail.PatientId = getBookHomeService.PatientId;
                        var patientDetail = _context.ApplicationUser.Where(a => a.Id == getBookHomeService.PatientId).FirstOrDefault();
                        if (patientDetail != null)
                        {
                            _getgAppointmentDetail.PatientFirstName = patientDetail.FirstName;
                            _getgAppointmentDetail.PatientLastName = patientDetail.LastName;
                            _getgAppointmentDetail.PatientEmail = patientDetail.Email;
                            _getgAppointmentDetail.PatientPhoneNumber = patientDetail.PhoneNumber;
                        }
                        _getgAppointmentDetail.DoctorId = getBookHomeService.DoctorId;
                        var doctorDetail = _context.ApplicationUser.Where(a => a.Id == getBookHomeService.DoctorId).FirstOrDefault();
                        if (doctorDetail != null)
                        {
                            _getgAppointmentDetail.DoctorFirstName = doctorDetail.FirstName;
                            _getgAppointmentDetail.DoctorLastName = doctorDetail.LastName;
                            _getgAppointmentDetail.DoctorEmail = doctorDetail.Email;
                            _getgAppointmentDetail.DoctorPhoneNumber = doctorDetail.PhoneNumber;
                        }

                        _getgAppointmentDetail.SlotFrom = getBookHomeService.FromTime;
                        // _getgAppointmentDetail.SlotTo = getBookHomeService.ToTime;

                        _getgAppointmentDetail.AppointmentDate = getBookHomeService.FromDate;
                        _getgAppointmentDetail.AppointmentStatus = getBookHomeService.Status;
                        _getgAppointmentDetail.appointmentTypeId = 2;
                        _getgAppointmentDetail.appointmentTypeName = "Home";
                        _getgAppointmentDetail.Fee = 0;
                    }
                }
                // Get Lab service Detail
                if (appointmentTypeId == 3)
                {
                    var bookLabServiceId = Convert.ToInt32(appointmentId);
                    var getBookLabService = _context.BookLabService.Where(a => a.BookLabServiceId == bookLabServiceId).FirstOrDefault();

                    if (getBookLabService != null)
                    {
                        _getgAppointmentDetail = new AdminAppointmentDetailsViewModel();
                        _getgAppointmentDetail.AppointmentId = getBookLabService.BookLabServiceId.ToString();
                        _getgAppointmentDetail.PatientId = getBookLabService.PatientId;
                        var patientDetail = _context.ApplicationUser.Where(a => a.Id == getBookLabService.PatientId).FirstOrDefault();
                        if (patientDetail != null)
                        {
                            _getgAppointmentDetail.PatientFirstName = patientDetail.FirstName;
                            _getgAppointmentDetail.PatientLastName = patientDetail.LastName;
                            _getgAppointmentDetail.PatientEmail = patientDetail.Email;
                            _getgAppointmentDetail.PatientPhoneNumber = patientDetail.PhoneNumber;
                        }
                        _getgAppointmentDetail.DoctorId = getBookLabService.DoctorId;
                        var doctorDetail = _context.ApplicationUser.Where(a => a.Id == getBookLabService.DoctorId).FirstOrDefault();
                        if (doctorDetail != null)
                        {
                            _getgAppointmentDetail.DoctorFirstName = doctorDetail.FirstName;
                            _getgAppointmentDetail.DoctorLastName = doctorDetail.LastName;
                            _getgAppointmentDetail.DoctorEmail = doctorDetail.Email;
                            _getgAppointmentDetail.DoctorPhoneNumber = doctorDetail.PhoneNumber;
                        }
                        _getgAppointmentDetail.SlotFrom = getBookLabService.FromTime;
                        _getgAppointmentDetail.AppointmentDate = getBookLabService.FromDate;
                        _getgAppointmentDetail.AppointmentStatus = getBookLabService.Status;
                        _getgAppointmentDetail.Fee = 0;
                        _getgAppointmentDetail.appointmentTypeId = 3;
                        _getgAppointmentDetail.appointmentTypeName = "Lab";
                    }
                }
                // Get Radiology service Detail
                if (appointmentTypeId == 5)
                {
                    var getBookRadiologyService = _context.BookRadiologyService.Where(a => a.BookRadiologyServiceId == Convert.ToInt32(appointmentId)).FirstOrDefault();

                    if (getBookRadiologyService != null)
                    {
                        _getgAppointmentDetail = new AdminAppointmentDetailsViewModel();
                        _getgAppointmentDetail.AppointmentId = getBookRadiologyService.BookRadiologyServiceId.ToString();
                        _getgAppointmentDetail.PatientId = getBookRadiologyService.PatientId;
                        var patientDetail = _context.ApplicationUser.Where(a => a.Id == getBookRadiologyService.PatientId).FirstOrDefault();
                        if (patientDetail != null)
                        {
                            _getgAppointmentDetail.PatientFirstName = patientDetail.FirstName;
                            _getgAppointmentDetail.PatientLastName = patientDetail.LastName;
                            _getgAppointmentDetail.PatientEmail = patientDetail.Email;
                            _getgAppointmentDetail.PatientPhoneNumber = patientDetail.PhoneNumber;
                        }

                        _getgAppointmentDetail.SlotFrom = getBookRadiologyService.BookingTime;
                        _getgAppointmentDetail.AppointmentDate = getBookRadiologyService.BookingDate;
                        _getgAppointmentDetail.AppointmentStatus = getBookRadiologyService.BookingStatus;
                        _getgAppointmentDetail.appointmentTypeId = 5;
                        _getgAppointmentDetail.appointmentTypeName = "Radiology";
                        _getgAppointmentDetail.Fee = 0;
                    }
                }
                // Get Dialysis service Detail
                if (appointmentTypeId == 4)
                {
                    var getBookDialysisService = _context.BookDialysisService.Where(a => a.BookDialysisServiceId == Convert.ToInt32(appointmentId)).FirstOrDefault();

                    if (getBookDialysisService != null)
                    {
                        _getgAppointmentDetail = new AdminAppointmentDetailsViewModel();
                        _getgAppointmentDetail.AppointmentId = getBookDialysisService.BookDialysisServiceId.ToString();
                        _getgAppointmentDetail.PatientId = getBookDialysisService.PatientId;
                        var patientDetail = _context.ApplicationUser.Where(a => a.Id == getBookDialysisService.PatientId).FirstOrDefault();
                        if (patientDetail != null)
                        {
                            _getgAppointmentDetail.PatientFirstName = patientDetail.FirstName;
                            _getgAppointmentDetail.PatientLastName = patientDetail.LastName;
                            _getgAppointmentDetail.PatientEmail = patientDetail.Email;
                            _getgAppointmentDetail.PatientPhoneNumber = patientDetail.PhoneNumber;
                        }
                        _getgAppointmentDetail.SlotFrom = getBookDialysisService.BookingTime;
                        _getgAppointmentDetail.AppointmentDate = getBookDialysisService.BookingDate;
                        _getgAppointmentDetail.AppointmentDate = getBookDialysisService.BookingDate;
                        _getgAppointmentDetail.AppointmentStatus = getBookDialysisService.BookingStatus;
                        _getgAppointmentDetail.appointmentTypeId = 4;
                        _getgAppointmentDetail.appointmentTypeName = "Dialysis";
                        _getgAppointmentDetail.Fee = 0;
                    }
                }
                // Get Rehabilation service Detail
                if (appointmentTypeId == 6)
                {
                    var getBookRehabilationService = _context.BookRehabilationService.Where(a => a.BookRehabilationServiceId == Convert.ToInt32(appointmentId)).FirstOrDefault();

                    if (getBookRehabilationService != null)
                    {
                        _getgAppointmentDetail = new AdminAppointmentDetailsViewModel();
                        _getgAppointmentDetail.AppointmentId = getBookRehabilationService.BookRehabilationServiceId.ToString();
                        _getgAppointmentDetail.PatientId = getBookRehabilationService.PatientId;
                        var patientDetail = _context.ApplicationUser.Where(a => a.Id == getBookRehabilationService.PatientId).FirstOrDefault();
                        if (patientDetail != null)
                        {
                            _getgAppointmentDetail.PatientFirstName = patientDetail.FirstName;
                            _getgAppointmentDetail.PatientLastName = patientDetail.LastName;
                            _getgAppointmentDetail.PatientEmail = patientDetail.Email;
                            _getgAppointmentDetail.PatientPhoneNumber = patientDetail.PhoneNumber;
                        }
                        _getgAppointmentDetail.SlotFrom = getBookRehabilationService.BookingTime;
                        _getgAppointmentDetail.AppointmentDate = getBookRehabilationService.BookingDate;
                        _getgAppointmentDetail.AppointmentDate = getBookRehabilationService.BookingDate;
                        _getgAppointmentDetail.AppointmentStatus = getBookRehabilationService.BookingStatus;
                        _getgAppointmentDetail.appointmentTypeId = 6;
                        _getgAppointmentDetail.appointmentTypeName = "Rehabilation";
                        _getgAppointmentDetail.Fee = 0;
                    }
                }


                if (_getgAppointmentDetail.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Pending)
                {
                    _getgAppointmentDetail.AppointmentStatusDisplay = "Pending";
                }
                // if (_getgAppointmentDetail.appointmentTypeId == 1)
                // {
                //     _getgAppointmentDetail.AppointmentDate = Convert.ToDateTime(_getgAppointmentDetail.AppointmentDate).ToString(@"dd-MM-yyyy");
                // }

                if (_getgAppointmentDetail.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Confirmed)
                {
                    _getgAppointmentDetail.AppointmentStatusDisplay = "Confirmed";
                }

                if (_getgAppointmentDetail.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Rejected)
                {
                    _getgAppointmentDetail.AppointmentStatusDisplay = "Rejected";
                }

                if (_getgAppointmentDetail.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Cancelled)
                {
                    _getgAppointmentDetail.AppointmentStatusDisplay = "Cancelled";
                }

                if (_getgAppointmentDetail != null)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        message = "Appointment detail " + ResponseMessages.msgShownSuccess,
                        data = _getgAppointmentDetail,
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgNoUpcomingAppointments,
                        data = new { },
                        code = StatusCodes.Status200OK
                    });
                }
            }
            else
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
            }
        }
        #endregion
    }
}