using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace AnfasAPI.Repositories
{
    public class NotificationService : INotificationService
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IContentService _contentService;
        private readonly IEmailSender _emailSenderService;
        public NotificationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IContentService contentService, IEmailSender emailSenderService)
        {
            _context = context;
            _userManager = userManager;
            _contentService = contentService;
            _emailSenderService = emailSenderService;
        }

        public bool CreateNotification(Notifications model)
        {
            try
            {
                _context.Notifications.Add(model);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateNotificationOnAppointmentStatusChange(Appointments appointment, string userId, string userRole)
        {
            try
            {
                
                var _user = await _userManager.FindByIdAsync(userId);
                var _doctor = await _userManager.FindByIdAsync(_user.Id);
                string doctorName = _doctor.FirstName + " " + _doctor.LastName;
                if (appointment != null)
                {
                    Notifications objNotifications = new Notifications();
                    if (userRole.ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower())
                    {
                        if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Confirmed)
                        {
                            objNotifications.FromUser = appointment.DoctorId;
                            objNotifications.ToUser = appointment.PatientId;
                            objNotifications.Type = (int)GlobalVariables.NotificationTypes.CONFIRMED_APPOINTMENT;
                            objNotifications.Text = NotificationMessages.GetAppointmentConfirmedNotificationMsg(doctorName);
                        }
                        else if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Rejected)
                        {
                            objNotifications.FromUser = appointment.DoctorId;
                            objNotifications.ToUser = appointment.PatientId;
                            objNotifications.Type = (int)GlobalVariables.NotificationTypes.REJECTED_APPOINTMENT;
                            objNotifications.Text = NotificationMessages.GetAppointmentRejectedNotificationMsg(doctorName);
                        }
                        else if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Cancelled)
                        {
                            objNotifications.FromUser = appointment.DoctorId;
                            objNotifications.ToUser = appointment.PatientId;
                            objNotifications.Type = (int)GlobalVariables.NotificationTypes.CANCELLED_APPOINTMENT;
                            objNotifications.Text = NotificationMessages.GetAppointmentCancelledNotificationMsg(doctorName);
                        }
                    }
                    else if (userRole.ToLower() == GlobalVariables.UserRole.Patient.ToString().ToLower())
                    {
                        var _patient = await _userManager.FindByIdAsync(userId);
                        string patientName = _patient.FirstName + " " + _patient.LastName;
                        if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Cancelled)
                        {
                            objNotifications.FromUser = appointment.PatientId;
                            objNotifications.ToUser = appointment.DoctorId;
                            objNotifications.Type = (int)GlobalVariables.NotificationTypes.CANCELLED_APPOINTMENT;
                            objNotifications.Text = NotificationMessages.GetAppointmentCancelledNotificationMsg(patientName);
                        }
                    }
                    else
                    {
                        return false;
                    }
                    objNotifications.IsRead = false;
                    objNotifications.CreatedOn = DateTime.UtcNow;
                    objNotifications.Purpose = (int)GlobalVariables.PurposeTypes.Appointment;
                    objNotifications.PurposeId = appointment.Id;
                    return CreateNotification(objNotifications);

                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> CreatePushNotificationOnAppointmentStatusChange(Appointments appointment, string userRole, DoctorTimeSlotsViewModel _timeSlotsModel)
        {
            try
            {
                bool result = false;
                if (appointment != null)
                {
                    var _patient = await _userManager.FindByIdAsync(appointment.PatientId);
                    var _patientName = _patient.FirstName + " " + _patient.LastName;
                    var _doctor = await _userManager.FindByIdAsync(appointment.DoctorId);
                    var _doctorName = _doctor.FirstName + " " + _doctor.LastName;
                    if (userRole.ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower())
                    {
                        var _userConfiguration = _contentService.GetUserConfigurations(appointment.PatientId);
                        if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Confirmed)
                        {
                            if (_userConfiguration.PushNotificationStatus)
                            {
                                string[] tokens = new string[] { _patient.DeviceToken };
                                var body = PushNotificationMessages.GetAppointmentConfirmedPushNotificationMsg(_doctorName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                                var data = new { my_custom_key = "Confirmed Appointment Push notification", title = PushNotificationMessages.appointmentConfirmedTitle, type = (int)GlobalVariables.NotificationTypes.CONFIRMED_APPOINTMENT, message = body};
                                result = await SendPushNotificationsForPatient(tokens, PushNotificationMessages.appointmentConfirmedTitle, body, data);
                            }
                            return result;
                        }
                        else if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Rejected)
                        {
                            if (_userConfiguration.PushNotificationStatus)
                            {
                                string[] tokens = new string[] { _patient.DeviceToken };
                                var body = PushNotificationMessages.GetAppointmentRejectedByDoctorPushNotificationMsg(_doctorName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                                var data = new { my_custom_key = "Rejected Appointment Push notification", title = PushNotificationMessages.appointmentRejectedTitle, type = (int)GlobalVariables.NotificationTypes.REJECTED_APPOINTMENT, message = body };
                                result = await SendPushNotificationsForPatient(tokens, PushNotificationMessages.appointmentRejectedTitle, body, data);
                            }
                            return result;
                        }
                        else
                        {
                            if (_userConfiguration.PushNotificationStatus)
                            {
                                string[] tokens = new string[] { _patient.DeviceToken };
                                var body = PushNotificationMessages.GetAppointmentCancelledByDoctorPushNotificationMsg(_doctorName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                                var data = new { my_custom_key = "Cancelled Appointment Push notification", title = PushNotificationMessages.appointmentCancelledTitle, type = (int)GlobalVariables.NotificationTypes.CANCELLED_APPOINTMENT, message = body };
                                result = await SendPushNotificationsForPatient(tokens, PushNotificationMessages.appointmentCancelledTitle, body, data);
                            }
                            return result;
                        }
                    }
                    else if (userRole.ToLower() == GlobalVariables.UserRole.Patient.ToString().ToLower())
                    {
                        var _userConfiguration = _contentService.GetUserConfigurations(appointment.DoctorId);

                        if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Cancelled)
                        {
                            if (_userConfiguration.PushNotificationStatus)
                            {
                                string[] tokens = new string[] { _doctor.DeviceToken };
                                var body = PushNotificationMessages.GetAppointmentCancelledByPatientPushNotificationMsg(_patientName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                                result = await SendPushNotification(tokens, PushNotificationMessages.appointmentCancelledTitle, body);
                            }
                            return result;
                        }

                    }
                    else
                    {
                        return result;
                    }
                }
                else
                {
                    return result;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FilterationResponseModel<PatientNotificationListViewModel> GetNotificationListByUser(FilterationListViewModel model, string userId)
        {
            try
            {
                if (_context != null)
                {
                    //Return Lists  
                    var source = (from n in _context.Notifications
                                  join au in _context.ApplicationUser
                                  on n.FromUser equals au.Id
                                  join gm in _context.GenderMaster
                                  on au.GenderId equals gm.Id
                                  where n.ToUser == userId && n.IsDeleted == false
                                  orderby n.CreatedOn descending
                                  select new PatientNotificationListViewModel
                                  {
                                      Id = n.Id,
                                      FromUser = n.FromUser,
                                      ToUser = n.ToUser,
                                      FromUserName = au.FirstName + ' ' + au.LastName,
                                      FromUserProfilePic = au.ProfilePic,
                                      FromUserGender = gm.Name,
                                      Type = n.Type,
                                      Text = n.Text,
                                      Purpose = n.Purpose,
                                      IsRead = n.IsRead,
                                      TimeInMinutes = CommonFunctions.GetTimeDifferenceInMinutes(DateTime.UtcNow, n.CreatedOn),
                                      CreatedOn = n.CreatedOn,
                                  }).AsQueryable();  //Filter Parameter With null checks

                    if (!string.IsNullOrEmpty(model.filterBy))
                    {
                        var filterBy = model.filterBy.ToLower();
                        source = source.Where(m => m.FromUserGender.ToLower().Contains(filterBy));
                    }

                    //Search Parameter With null checks   
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x => x.FromUserName.ToLower().Contains(search));
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
                    FilterationResponseModel<PatientNotificationListViewModel> objList = new FilterationResponseModel<PatientNotificationListViewModel>();
                    objList.totalCount = TotalCount;
                    objList.pageSize = PageSize;
                    objList.currentPage = CurrentPage;
                    objList.totalPages = TotalPages;
                    objList.previousPage = previousPage;
                    objList.nextPage = nextPage;
                    objList.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                    objList.dataList = items.OrderByDescending(x => x.CreatedOn).ToList();
                    return objList;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FilterationResponseModel<NotificationListViewModel> GetNotificationListForDoctor(FilterationListViewModel model, string userId)
        {
            try
            {
                if (_context != null)
                {
                    //Return Lists  
                    var source = _context.Notifications
                                .Join(_context.Appointments, ns => ns.PurposeId, ap => ap.Id, (ns, ap) => new { ns, ap })
                                .Join(_context.DoctorTimeSlots, dss => dss.ap.TimeSlotId, dts => dts.Id, (dss, dts) => new { dss, dts })
                                .Join(_context.ApplicationUser, auu => auu.dss.ns.FromUser, au => au.Id, (auu, au) => new { auu, au })
                                .Join(_context.GenderMaster, gmm => gmm.au.GenderId, gm => gm.Id, (gmm, gm) => new { gmm, gm })
                                .Where(x => x.gmm.auu.dss.ns.ToUser == userId)
                                .Where(x => x.gmm.auu.dss.ns.IsDeleted == false)
                                .AsEnumerable()
                                .Where(x => !(
                                              (x.gmm.auu.dss.ns.Type == 1 && x.gmm.auu.dss.ap.Status == 0
                                              && Convert.ToDateTime(x.gmm.auu.dts.Date.ToString(@"MM-dd-yyyy") + " " + x.gmm.auu.dts.SlotTo.ToString(@"hh\:mm\:ss")) < DateTime.UtcNow)
                                                                          ||
                                             (x.gmm.auu.dss.ap.Status == (int)GlobalVariables.AppointmentStatus.Confirmed ||
                                             x.gmm.auu.dss.ap.Status == (int)GlobalVariables.AppointmentStatus.Cancelled ||
                                             x.gmm.auu.dss.ap.Status == (int)GlobalVariables.AppointmentStatus.Rejected)
                                           )
                                      )
                                .OrderByDescending(x => x.gmm.auu.dss.ns.CreatedOn)
                                .Select(x => new NotificationListViewModel
                                {
                                    Id = x.gmm.auu.dss.ns.Id,
                                    FromUser = x.gmm.auu.dss.ns.FromUser,
                                    ToUser = x.gmm.auu.dss.ns.ToUser,
                                    FromUserName = x.gmm.au.FirstName + ' ' + x.gmm.au.LastName,
                                    FromUserProfilePic = x.gmm.au.ProfilePic,
                                    FromUserGender = x.gm.Name,
                                    Type = x.gmm.auu.dss.ns.Type,
                                    Text = x.gmm.auu.dss.ns.Text,
                                    Purpose = x.gmm.auu.dss.ns.Purpose,
                                    PurposeId = x.gmm.auu.dss.ns.PurposeId,
                                    AppointmentStatus = x.gmm.auu.dss.ap.Status,
                                    TimeSlotId = x.gmm.auu.dss.ap.TimeSlotId,
                                    Date = x.gmm.auu.dts.Date.ToString(GlobalVariables.DefaultDateFormat),
                                    SlotFrom = x.gmm.auu.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.gmm.auu.dts.SlotFrom),
                                    SlotTo = x.gmm.auu.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.gmm.auu.dts.SlotTo),
                                    IsRead = x.gmm.auu.dss.ns.IsRead,
                                    TimeInMinutes = CommonFunctions.GetTimeDifferenceInMinutes(DateTime.UtcNow, x.gmm.auu.dss.ns.CreatedOn),
                                    CreatedOn = x.gmm.auu.dss.ns.CreatedOn
                                 
                                }).Distinct().AsQueryable(); //Filter Parameter With null checks


                    if (!string.IsNullOrEmpty(model.filterBy))
                    {
                        var filterBy = model.filterBy.ToLower();
                        source = source.Where(m => m.FromUserGender.ToLower().Contains(filterBy));
                    }

                    //Search Parameter With null checks   
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x => x.FromUserName.ToLower().Contains(search));
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
                    FilterationResponseModel<NotificationListViewModel> objList = new FilterationResponseModel<NotificationListViewModel>();
                    objList.totalCount = TotalCount;
                    objList.pageSize = PageSize;
                    objList.currentPage = CurrentPage;
                    objList.totalPages = TotalPages;
                    objList.previousPage = previousPage;
                    objList.nextPage = nextPage;
                    objList.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                    objList.dataList = items.OrderByDescending(x => x.CreatedOn).ToList();
                    return objList;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool MarkNotificationAsRead(string notificationId, string userId)
        {
            try
            {
                var notification = _context.Notifications.FirstOrDefault(c => c.Id == notificationId && c.ToUser == userId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    _context.Notifications.Update(notification);
                    _context.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteNotification(string NotificationId)
        {
            try
            {
                var _notification = _context.Notifications.Where(x => x.Id == NotificationId && x.IsDeleted == false).FirstOrDefault();
                _notification.IsDeleted = true;
                _context.Notifications.Update(_notification);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteAllNotifications(string userId)
        {
            try
            {
                var _notificationList = _context.Notifications.Where(x => x.ToUser == userId && x.IsDeleted == false).ToList();
                foreach (var item in _notificationList)
                {
                    var _notification = _context.Notifications.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (_notification != null)
                    {
                        _notification.IsDeleted = true;
                        _context.Notifications.Update(_notification);
                    }
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendPushNotification(string[] deviceTokens, string title, string body)
        {
            try
            {
                bool sent = false;
                if (deviceTokens.Count() > 0)
                {
                    // object Creation
                    var messageInformation = new FirebaseNotificationsMessageViewModel()
                    {

                        registration_ids = deviceTokens,
                        notification = new FirebaseNotificationsViewModel()
                        {
                            title = title,
                            body = body
                        }
                    };
                    string jsonMessage = JsonConvert.SerializeObject(messageInformation);
                    // Create request to FirebaseAPI
                    var request = new HttpRequestMessage(HttpMethod.Post, GlobalVariables.FireBasePushNotificationsURL);
                    request.Headers.TryAddWithoutValidation("Authorization", "Key=" + GlobalVariables.FirebaseServerKeyForDoctor);
                    request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    using (var Client = new HttpClient())
                    {
                        HttpResponseMessage result = await Client.SendAsync(request);
                        if (result.IsSuccessStatusCode)
                        {
                            sent = true;
                        }
                    }

                }
                return sent;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendPushNotificationsForDoctor(string[] deviceTokens, string title, string body, object data)
        {
            try
            {
                bool sent = false;
                if (deviceTokens.Count() > 0)
                {
                    // object Creation
                    var messageInformation = new FirebaseNotificationsMessageViewModel()
                    {

                        registration_ids = deviceTokens,
                        notification = new FirebaseNotificationsViewModel()
                        {
                            title = title,
                            body = body
                        },
                        data = data

                    };
                    string jsonMessage = JsonConvert.SerializeObject(messageInformation);
                    // Create request to FirebaseAPI
                    var request = new HttpRequestMessage(HttpMethod.Post, GlobalVariables.FireBasePushNotificationsURL);
                    request.Headers.TryAddWithoutValidation("Authorization", "Key=" + GlobalVariables.FirebaseServerKeyForDoctor);
                    request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    using (var Client = new HttpClient())
                    {
                        HttpResponseMessage result = await Client.SendAsync(request);
                        if (result.IsSuccessStatusCode)
                        {
                            sent = true;
                        }
                    }

                }
           
                return sent;
             }
            catch
            {
                return false;
            }
        }
        public async Task<bool> SendPushNotificationsForPatient(string[] deviceTokens, string title, string body, object data)
        {
            try
            {
                bool sent = false;
                if (deviceTokens.Count() > 0)
                {
                    // object Creation
                    var messageInformation = new FirebaseNotificationsMessageViewModel()
                    {

                        registration_ids = deviceTokens,
                        notification = new FirebaseNotificationsViewModel()
                        {
                            title = title,
                            body = body
                        },
                        data = data

                    };
                    string jsonMessage = JsonConvert.SerializeObject(messageInformation);
                    // Create request to FirebaseAPI
                    var request = new HttpRequestMessage(HttpMethod.Post, GlobalVariables.FireBasePushNotificationsURL);
                    request.Headers.TryAddWithoutValidation("Authorization", "Key=" + GlobalVariables.FirebaseServerKeyForPatient);
                    request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    using (var Client = new HttpClient())
                    {
                        HttpResponseMessage result = await Client.SendAsync(request);
                        if (result.IsSuccessStatusCode)
                        {
                            sent = true;
                        }
                    }

                }

                return sent;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendEmailOnAppointmentStatusChange(Appointments appointment, string userRole, DoctorTimeSlotsViewModel _timeSlotsModel)
    {
        try
        {
            if (appointment != null)
            {
                var _patient = await _userManager.FindByIdAsync(appointment.PatientId);
                var _patientName = _patient.FirstName + " " + _patient.LastName;
                var _doctor = await _userManager.FindByIdAsync(appointment.DoctorId);
                var _doctorName = _doctor.FirstName + " " + _doctor.LastName;
                if (userRole.ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower())
                {
                    var _userConfiguration = _contentService.GetUserConfigurations(appointment.PatientId);

                    if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Confirmed)
                    {
                        if (_userConfiguration.EmailNotificationStatus)
                        {
                            var _emailMsg = EmailMessages.GetAppointmentConfirmedEmailMsg(_doctorName, _patientName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                            await _emailSenderService.SendEmailAsync(email: _patient.Email, subject: EmailMessages.GetAppointmentConfirmedEmailSubject(_doctorName), message: _emailMsg);

                        }
                        return true;
                    }
                    else if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Rejected)
                    {
                        if (_userConfiguration.EmailNotificationStatus)
                        {
                            var _emailMsg = EmailMessages.GetAppointmentRejectedByDoctorEmailMsg(_doctorName, _patientName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                            await _emailSenderService.SendEmailAsync(email: _patient.Email, subject: EmailMessages.GetAppointmentRejectedByDoctorEmailSubject(_doctorName), message: _emailMsg);

                        }
                        return true;
                    }
                    else
                    {
                        //cancelled 
                        if (_userConfiguration.EmailNotificationStatus)
                        {
                            var _emailMsg = EmailMessages.GetAppointmentCancelledByDoctorEmailMsg(_doctorName, _patientName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                            await _emailSenderService.SendEmailAsync(email: _patient.Email, subject: EmailMessages.GetAppointmentCancelledByDoctorEmailSubject(_doctorName), message: _emailMsg);
                        }
                        return true;
                    }
                }
                else if (userRole.ToLower() == GlobalVariables.UserRole.Patient.ToString().ToLower())
                {
                    var _userConfiguration = _contentService.GetUserConfigurations(appointment.DoctorId);

                    if (appointment.Status == (int)GlobalVariables.AppointmentStatus.Cancelled)
                    {
                        if (_userConfiguration.EmailNotificationStatus)
                        {
                            var _emailMsg = EmailMessages.GetAppointmentCancelledByPatientEmailNotificationMsg(_doctorName, _patientName, _timeSlotsModel.SlotFrom, _timeSlotsModel.SlotTo, _timeSlotsModel.Date);
                            await _emailSenderService.SendEmailAsync(email: _doctor.Email, subject: EmailMessages.GetAppointmentCancelledByPatientEmailSubject(_patientName), message: _emailMsg);

                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
            return true;
        }

        catch (Exception)
        {
            throw;
        }
    }

    public List<DoctorPatientUpcomingAppointmentViewModel> GetUpcomingAppointmentListForJob()
    {
        var _upcomingAppointmentList = _context.Appointments
                            .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                            .Join(_context.ApplicationUser, app => app.ap.DoctorId, au => au.Id, (app, au) => new { app, au })
                            .Where(x => x.au.ApplicationStatus == 2)
                            .Where(x => x.app.ap.Status == 1)
                            .Where(x => x.au.IsDeleted == false)
                            .Where(x => x.app.ap.IsDeleted == false)
                            .AsEnumerable()
                            .Where(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " + x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")) >= DateTime.UtcNow)
                            .Select(x => new DoctorPatientUpcomingAppointmentViewModel
                            {
                                AppointmentId = x.app.ap.Id,
                                DoctorId = x.app.ap.DoctorId,
                                PatientId = x.app.ap.PatientId,
                                FirstName = x.au.FirstName,
                                LastName = x.au.LastName,
                                Email = x.au.Email,
                                Date = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat),
                                SlotFrom = CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotFrom),
                                SlotTo = CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotTo),
                                EstimatedTimeInMinutes = CommonFunctions.CalculateEstimatedTimeInMinutesUTC(x.app.dts.Date, x.app.dts.SlotFrom)
                            }).ToList();
        return _upcomingAppointmentList;

    }


    public async Task SendPushNotificationForCallReminder()
    {
        var dataList = GetUpcomingAppointmentListForJob();
        if (dataList.Count > 0)
        {
            var newList = dataList.Where(x => x.EstimatedTimeInMinutes > 0 && x.EstimatedTimeInMinutes <= 5).ToList();
            foreach (var item in newList)
            {
                var appointment = _context.Appointments.FirstOrDefault(c => c.Id == item.AppointmentId);
                if (appointment != null)
                {
                    var _patientConfiguration = _contentService.GetUserConfigurations(appointment.PatientId);
                    var _doctorConfiguration = _contentService.GetUserConfigurations(appointment.DoctorId);
                    var _patient = await _userManager.FindByIdAsync(appointment.PatientId);
                    var _doctor = await _userManager.FindByIdAsync(appointment.DoctorId);
                    var _doctorName = _doctor.FirstName + " " + _doctor.LastName;
                    var _patientName = _patient.FirstName + " " + _patient.LastName;
                    // Call reminder alert for patient
                    if (_patientConfiguration.PushNotificationStatus)
                    {
                        string[] tokens = new string[] { _patient.DeviceToken };
                        var body = PushNotificationMessages.GetAppointmentReminderToPatientPushNotificationMsg(_doctorName, item.SlotFrom, item.SlotTo, item.Date);
                        var data = new { my_custom_key = "Call Reminder Push notification", title = PushNotificationMessages.appointmentReminderTitle, type = (int)GlobalVariables.NotificationTypes.CALL_REMIND_ALERT, message = body };
                        await SendPushNotificationsForPatient(tokens, PushNotificationMessages.appointmentReminderTitle, body, data);
                    }
                    // Call reminder alert for doctor 
                    if (_doctorConfiguration.PushNotificationStatus)
                    {
                        string[] tokens = new string[] { _doctor.DeviceToken };
                        var body = PushNotificationMessages.GetAppointmentReminderToDoctorPushNotificationMsg(_patientName, item.SlotFrom, item.SlotTo, item.Date);
                        var data = new { my_custom_key = "Call Reminder Push notification", title = PushNotificationMessages.appointmentReminderTitle, type = (int)GlobalVariables.NotificationTypes.CALL_REMIND_ALERT, message = body };
                        await SendPushNotificationsForDoctor(tokens, PushNotificationMessages.appointmentReminderTitle, body, data);
                    }

                }
            }
        }
    }


}
}
