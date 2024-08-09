using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Data;


namespace AnfasAPI.Repositories
{
    public class AppointmentService : IAppointmentService
    {
        private ApplicationDbContext _context;
        private readonly IScheduleService _scheduleService;
        private readonly INotificationService _notificationService;
        private readonly IContentService _contentService;
        private UserManager<ApplicationUser> _userManager;
        public AppointmentService(ApplicationDbContext context, IScheduleService scheduleService, INotificationService notificationService, IContentService contentService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _scheduleService = scheduleService;
            _notificationService = notificationService;
            _contentService = contentService;
            _userManager = userManager;

        }
        public async Task<ResponseBookAppointmentViewModel> BookAppointment(Appointments model)
        {
            ResponseBookAppointmentViewModel objModel = new ResponseBookAppointmentViewModel();
            try
            {
                int preBookingTime = GlobalVariables.preBookingTimeInMinutes;
                bool isAlreadyExists = _context.Appointments.Any(x => x.TimeSlotId == model.TimeSlotId && x.DoctorId == model.DoctorId && x.IsDeleted == false);
                var _appointmentDetails = _context.DoctorTimeSlots.Where(x => x.Id == model.TimeSlotId).FirstOrDefault();
               bool isAlreadyBooked = CheckForSameDateSlot(model.PatientId, _appointmentDetails.Date, _appointmentDetails.SlotFrom);
               int EstimatedTimeInMinUTC = CommonFunctions.CalculateEstimatedTimeInMinutesUTC(_appointmentDetails.Date, _appointmentDetails.SlotFrom);
                if (isAlreadyExists) // appointment cannot be booked for same time slot 
                {
                    objModel.Status = false;
                    objModel.ResponseValue = 0;
                    return objModel;
                }
                else if (isAlreadyBooked) // appointment cannot be booked for same date and time for another doctor
                {

                    objModel.Status = false;
                    objModel.ResponseValue = -3;
                    return objModel;

                }
                else if (EstimatedTimeInMinUTC > 0 && EstimatedTimeInMinUTC <= preBookingTime)
                {
                    objModel.Status = false;   // appointments can not be booked before 15 min. as per utc time
                    objModel.ResponseValue = -1;
                    return objModel;
                }
                else if(Convert.ToDateTime(_appointmentDetails.Date.ToString(@"MM-dd-yyyy") + " " +
                                                           _appointmentDetails.SlotFrom.ToString(@"hh\:mm\:ss")
                                  ) < DateTime.UtcNow)
                {
                    Console.WriteLine(Convert.ToDateTime(_appointmentDetails.Date.ToString(@"MM-dd-yyyy") + " " +
                                                           _appointmentDetails.SlotFrom.ToString(@"hh\:mm\:ss")));
                    Console.WriteLine(DateTime.UtcNow);
                    // change date format to "dd-MM-yyyy" for dev environment
                    // appointment cannot be booked if current utc time has passed 
                    objModel.Status = false;  
                    objModel.ResponseValue = -2;
                    return objModel;
                }
                //else if (CommonFunctions.ConvertToDateTime2(_appointmentDetails.Date, _appointmentDetails.SlotFrom) < DateTime.UtcNow)
                //{
                //    objModel.Status = false;
                //    objModel.ResponseValue = -2;
                //    return objModel;
                //}

                else
                {
                    _context.Appointments.Add(model);
                    await _context.SaveChangesAsync();
                    objModel.Status = true;
                    objModel.ResponseValue = 1;
                    objModel.AppointmentId = model.Id;
                    objModel.SlotFrom = CommonFunctions.ConvertTimeSpanToString(_appointmentDetails.SlotFrom);
                    objModel.SlotTo = CommonFunctions.ConvertTimeSpanToString(_appointmentDetails.SlotTo);
                    objModel.Date = _appointmentDetails.Date.ToString(GlobalVariables.DefaultDateFormat);
                    return objModel;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int> UpdateAppointmentStatus(string userId, string userRole, AppointmentStatusChangeViewModel model)
        {
            try
            {
                var appointment = await _context.Appointments.Where(c => c.Id == model.AppointmentId).FirstOrDefaultAsync();
                var _doctorTimeSlotDetails = await _context.DoctorTimeSlots.Where(s => s.Id == appointment.TimeSlotId).FirstOrDefaultAsync();
                int EstimatedTimeInMinUTC = CommonFunctions.CalculateEstimatedTimeInMinutesUTC(_doctorTimeSlotDetails.Date, _doctorTimeSlotDetails.SlotFrom);
                DoctorTimeSlotsViewModel _timeSlotsModel = new DoctorTimeSlotsViewModel();
                _timeSlotsModel.SlotFrom = CommonFunctions.ConvertTimeSpanToString(_doctorTimeSlotDetails.SlotFrom);
                _timeSlotsModel.SlotTo = CommonFunctions.ConvertTimeSpanToString(_doctorTimeSlotDetails.SlotTo);
                _timeSlotsModel.Date = _doctorTimeSlotDetails.Date.ToString(GlobalVariables.DefaultDateFormat);
                if (appointment != null)
                {
                    if (model.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Confirmed)
                    {
                        if (EstimatedTimeInMinUTC > 0 && EstimatedTimeInMinUTC <= GlobalVariables.preBookingConfirmedTimeInMinutes)
                        {
                            return -1; // 15 min. check 
                        }
                        else
                        {
                            // appointment confirmed 
                            appointment.Status = (int)GlobalVariables.AppointmentStatus.Confirmed;
                            appointment.UpdateDate = DateTime.UtcNow;
                            _context.Appointments.Update(appointment);
                            _context.SaveChanges();
                            await _notificationService.CreateNotificationOnAppointmentStatusChange(appointment, userId, userRole);
                            await _notificationService.CreatePushNotificationOnAppointmentStatusChange(appointment, userRole, _timeSlotsModel);
                            await _notificationService.SendEmailOnAppointmentStatusChange(appointment, userRole, _timeSlotsModel);
                            return 1;
                        }

                    }
                    else if (model.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Rejected || model.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Cancelled)
                    {
                        if (EstimatedTimeInMinUTC > 0 && EstimatedTimeInMinUTC <= GlobalVariables.preBookingCancelTimeInMinutes)
                        {
                            return -2;// 1 hour check 
                        }
                        else
                        {
                            // appointment rejected or cancelled
                            bool status = await _scheduleService.UpdateTimeSlotStatus(appointment.TimeSlotId, true);
                            if (status)
                            {
                                DeleteAppointment(appointment.Id, userId, model.AppointmentStatus);
                            }
                            await _notificationService.CreateNotificationOnAppointmentStatusChange(appointment, userId, userRole);
                            await _notificationService.CreatePushNotificationOnAppointmentStatusChange(appointment, userRole, _timeSlotsModel);
                            await _notificationService.SendEmailOnAppointmentStatusChange(appointment, userRole, _timeSlotsModel);
                            return 1;
                        }

                    }
                    else if (model.AppointmentStatus == (int)GlobalVariables.AppointmentStatus.Completed)
                    {
                        // appointment status - completed
                        //if (CommonFunctions.ConvertToDateTime2(_doctorTimeSlotDetails.Date, _doctorTimeSlotDetails.SlotFrom) <= DateTime.UtcNow
                        //    && DateTime.UtcNow <= CommonFunctions.ConvertToDateTime2(_doctorTimeSlotDetails.Date, _doctorTimeSlotDetails.SlotTo)
                        //    )
                        /* Use date format "dd-MM-yyyy" for dev environment */
                       if (Convert.ToDateTime(_doctorTimeSlotDetails.Date.ToString(@"MM-dd-yyyy") + " " + _doctorTimeSlotDetails.SlotFrom.ToString(@"hh\:mm\:ss")) <= DateTime.UtcNow
                            && DateTime.UtcNow <= Convert.ToDateTime(_doctorTimeSlotDetails.Date.ToString(@"MM-dd-yyyy") + " " + _doctorTimeSlotDetails.SlotTo.ToString(@"hh\:mm\:ss"))
                            )
                        {
                            
                            bool status = await _scheduleService.UpdateTimeSlotStatus(appointment.TimeSlotId, true);
                            if (status)
                            {
                                DeleteAppointment(appointment.Id, userId, model.AppointmentStatus);
                            }
                            await _notificationService.CreateNotificationOnAppointmentStatusChange(appointment, userId, userRole);
                            await _notificationService.CreatePushNotificationOnAppointmentStatusChange(appointment, userRole, _timeSlotsModel);
                            await _notificationService.SendEmailOnAppointmentStatusChange(appointment, userRole, _timeSlotsModel);
                            return 1;

                        }
                        else
                        {
                            // appointment can not be completed before appointment time 
                            return -3;
                        }

                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool DeleteAppointment(string appointmentId, string userId, int appointmentStatus)
        {
            try
            {
                var appointment = _context.Appointments.FirstOrDefault(c => c.Id == appointmentId);
                if (appointment != null)
                {
                    appointment.Status = appointmentStatus;
                    appointment.IsDeleted = true;
                    appointment.DeletedBy = userId;
                    appointment.DeletedDate = DateTime.UtcNow;
                    _context.Appointments.Update(appointment);
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
        // Patient Upcoming Appointment
        public List<PatientUpcomingAppointmentViewModel> GetPatientUpcomingAppointments(string userId, int appointmentStatus)
        {
            try
            {
                //Use Date time format "dd-MM-yyyy"  for dev environment
                var _appointmentList = _context.Appointments
                             .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                             .Join(_context.ApplicationUser, app => app.ap.DoctorId, au => au.Id, (app, au) => new { app, au })
                             .Where(x => x.app.ap.Status == appointmentStatus)
                             .Where(x => x.app.ap.IsDeleted == false)
                             .Where(x => x.app.ap.PatientId == userId)
                             .AsEnumerable()
                             .Where(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                            x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")
                                   ) >= DateTime.UtcNow)
                              .OrderBy(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                              x.app.dts.SlotFrom.ToString(@"hh\:mm\:ss")
                                               ))
                             //.Where(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotTo) >= DateTime.UtcNow)
                             //.OrderBy(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotFrom))
                             .Select(x => new PatientUpcomingAppointmentViewModel
                             {
                                 AppointmentId = x.app.ap.Id,
                                 PatientId = x.app.ap.PatientId,
                                 DoctorId = x.app.ap.DoctorId,
                                 Dr_FirstName = x.au.FirstName,
                                 Dr_LastName = x.au.LastName,
                                 Dr_ProfilePic = x.au.ProfilePic,
                                 DoctorSpecialities = (from ds in _context.DoctorSpecialityInfo
                                                       join sm in _context.SpecialityMaster
                                                       on ds.SpecialityId equals sm.Id
                                                       where ds.UserId == x.app.ap.DoctorId
                                                       select new SpecialityInfoViewModel
                                                       {
                                                           Id = ds.SpecialityId,
                                                           Name = sm.Name,
                                                       }).ToList(),
                                 TimeSlotId = x.app.ap.TimeSlotId,
                                 SlotFrom = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotFrom),
                                 SlotTo = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotTo),
                                 Date = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat),
                                 AppointmentStatus = x.app.ap.Status,
                                 PaymentStatus = (int)GlobalVariables.PaymentStatus.Complete

                             }).ToList();

                return _appointmentList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public FilterationResponseModel<PatientPastAppointmentViewModel> GetPatientPastAppointments(FilterationListViewModel model, string userId)
        {
            try
            {
                //Use Date time format "dd-MM-yyyy"  for dev environment
                if (_context != null)
                {
                    var source = _context.Appointments
                               .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                               .Join(_context.ApplicationUser, app => app.ap.DoctorId, au => au.Id, (app, au) => new { app, au })
                               .Where(x => x.app.ap.IsDeleted == false)
                               .Where(x => x.app.ap.PatientId == userId)
                               .AsEnumerable()
                               //.Where(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotTo) <= DateTime.UtcNow)
                               //.OrderBy(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotFrom))
                               .Where(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                              x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")
                                     ) <= DateTime.UtcNow)
                               .OrderByDescending(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                              x.app.dts.SlotFrom.ToString(@"hh\:mm\:ss")
                                               ))
                               
                               .Select(x => new PatientPastAppointmentViewModel
                               {
                                   AppointmentId = x.app.ap.Id,
                                   PatientId = x.app.ap.PatientId,
                                   DoctorId = x.app.ap.DoctorId,
                                   Dr_FirstName = x.au.FirstName,
                                   Dr_LastName = x.au.LastName,
                                   Dr_ProfilePic = x.au.ProfilePic,
                                   DoctorSpecialities = (from ds in _context.DoctorSpecialityInfo
                                                         join sm in _context.SpecialityMaster
                                                         on ds.SpecialityId equals sm.Id
                                                         where ds.UserId == x.app.ap.DoctorId
                                                         select new SpecialityInfoViewModel
                                                         {
                                                             Id = ds.SpecialityId,
                                                             Name = sm.Name,
                                                         }).ToList(),
                                   TimeSlotId = x.app.ap.TimeSlotId,
                                   SlotFrom = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotFrom),
                                   SlotTo = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotTo),
                                   Date = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat),
                                   AppointmentStatus = x.app.ap.Status,
                                   PaymentStatus = (int)GlobalVariables.PaymentStatus.Complete,
                                   Rating = 5
                               }).AsQueryable();

                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x => x.Dr_FirstName.ToLower().Contains(search));
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
                    FilterationResponseModel<PatientPastAppointmentViewModel> obj = new FilterationResponseModel<PatientPastAppointmentViewModel>();
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
            catch (Exception)
            {
                throw;
            }
        }

        // Doctor Upcoming Appointments (Pending or Confirmed)

        public FilterationResponseModel<DoctorUpcomingAppointmentsViewModel> GetDoctorUpcomingAppointments(FilterationListViewModel model, string userId, int appointmentStatus)
        {
            try
            {
                //Use Date time format "dd-MM-yyyy"  for dev environment
                if (_context != null)
                {

                    var source = _context.Appointments
                                .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                                .Join(_context.ApplicationUser, app => app.ap.PatientId, au => au.Id, (app, au) => new { app, au })
                                .Where(x => x.app.ap.Status == appointmentStatus)
                                .Where(x => x.app.ap.IsDeleted == false)
                                .Where(x => x.app.ap.DoctorId == userId)
                                .AsEnumerable()
                                .Where(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                           x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")
                                  ) >= DateTime.UtcNow)
                                .OrderBy(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                            x.app.dts.SlotFrom.ToString(@"hh\:mm\:ss")
                                             ))
                                //.Where(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotTo) >= DateTime.UtcNow)
                                //.OrderBy(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotFrom))
                                .Select(x => new DoctorUpcomingAppointmentsViewModel
                                {
                                    PatientId = x.app.ap.PatientId,
                                    PatientName = x.au.FirstName + " " + x.au.LastName,
                                    PatientProfilePic = x.au.ProfilePic,
                                    AppointmentId = x.app.ap.Id,
                                    AppointmentStatus = x.app.ap.Status,
                                    TimeSlotId = x.app.ap.TimeSlotId,
                                    SlotFrom = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotFrom),
                                    SlotTo = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotTo),
                                    Date = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat)

                                }).AsQueryable();


                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x => x.PatientName.ToLower().Contains(search));
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
                    FilterationResponseModel<DoctorUpcomingAppointmentsViewModel> obj = new FilterationResponseModel<DoctorUpcomingAppointmentsViewModel>();
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
            catch (Exception)
            {
                throw;
            }
        }

        // Doctor Past Appointments (Rejected, Cancelled or Completed)
        public FilterationResponseModel<DoctorPastAppointmentsViewModel> GetDoctorPastAppointments(FilterationListViewModel model, string userId)
        {
            try
            {
                //Use Date time format "dd-MM-yyyy"  for dev environment
                if (_context != null)
                {
                    var source = _context.Appointments
                                .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                                .Join(_context.ApplicationUser, app => app.ap.PatientId, au => au.Id, (app, au) => new { app, au })
                                .Where(x => x.app.ap.DoctorId == userId)
                                .AsEnumerable()
                                .Where(x => (x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Completed ||
                                             x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Cancelled ||
                                             x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Rejected)
                                                                    ||
                                         ((x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Pending
                                          || x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Confirmed)
                                          &&(Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                                x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")
                                                               )  < DateTime.UtcNow)
                                         )
                                          //&& (CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotTo) < DateTime.UtcNow))
                                      )
                                .OrderByDescending(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                              x.app.dts.SlotFrom.ToString(@"hh\:mm\:ss")
                                               ))
                                //.OrderByDescending(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotFrom))

                                .Select(x => new DoctorPastAppointmentsViewModel
                                {
                                    PatientId = x.app.ap.PatientId,
                                    PatientName = x.au.FirstName + " " + x.au.LastName,
                                    PatientProfilePic = x.au.ProfilePic,
                                    AppointmentId = x.app.ap.Id,
                                    AppointmentStatus = x.app.ap.Status,
                                    TimeSlotId = x.app.ap.TimeSlotId,
                                    SlotFrom = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotFrom),
                                    SlotTo = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.app.dts.SlotTo),
                                    Date = x.app.dts.Date.ToString(GlobalVariables.DefaultDateFormat)
                                }).AsQueryable();

                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x => x.PatientName.ToLower().Contains(search));
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

                    //Returns List of Customer after applying Paging
                    var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    // if CurrentPage is greater than 1 means it has previousPage  
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";

                    // if TotalPages is greater than CurrentPage means it has nextPage  
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    // Returing List of Customers Collections  
                    FilterationResponseModel<DoctorPastAppointmentsViewModel> obj = new FilterationResponseModel<DoctorPastAppointmentsViewModel>();
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
            catch (Exception)
            {
                throw;
            }

        }

        public AppointmentDetailsViewModel GetAppointmentById(string id)
        {
            return (from ap in _context.Appointments
                    join dts in _context.DoctorTimeSlots
                    on ap.TimeSlotId equals dts.Id
                    where ap.IsDeleted == false && ap.Status == (int)GlobalVariables.AppointmentStatus.Confirmed
                    && ap.Id == id
                    select new AppointmentDetailsViewModel
                    {
                        AppointmentId = ap.Id,
                        PatientId = ap.PatientId,
                        DoctorId = ap.DoctorId,
                        TimeSlotId = ap.TimeSlotId,
                        SlotFrom = CommonFunctions.ConvertTimeSpanToString(dts.SlotFrom),
                        SlotTo = CommonFunctions.ConvertTimeSpanToString(dts.SlotTo),
                        Date = dts.Date.ToString(GlobalVariables.DefaultDateFormat),
                        AppointmentStatus = ap.Status,
                        Fee = ap.Fee,
                        IsSlotAvailable = dts.IsSlotAvailable
                    }).FirstOrDefault();
        }

        public int TotalUpcomingAppointments(string userId)
        {
            try
            {
                //Use Date time format "dd-MM-yyyy"  for dev environment
                if (_context != null)
                {
                    var source = _context.Appointments
                                .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                                .Join(_context.ApplicationUser, app => app.ap.PatientId, au => au.Id, (app, au) => new { app, au })
                                .Where(x => x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Confirmed)
                                .Where(x => x.app.ap.IsDeleted == false)
                                .Where(x => x.app.ap.DoctorId == userId)
                                .AsEnumerable()
                                .Where(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                            x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")
                                   ) >= DateTime.UtcNow)
                                //.Where(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotTo) >= DateTime.UtcNow)
                                .ToList();

                    // Get's No of Rows Count   
                    int TotalCount = source.Count();
                    // Display TotalCount to Records to User  
                    return TotalCount;
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int TotalPendingAppointments(string userId)
        {
            try
            {
                //Use Date time format "dd-MM-yyyy"  for dev environment
                if (_context != null)
                {
                    var source = _context.Appointments
                                .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                                .Join(_context.ApplicationUser, app => app.ap.PatientId, au => au.Id, (app, au) => new { app, au })
                                .Where(x => x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Pending)
                                .Where(x => x.app.ap.IsDeleted == false)
                                .Where(x => x.app.ap.DoctorId == userId)
                                .AsEnumerable()
                                .Where(x => Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                            x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")
                                   ) >= DateTime.UtcNow)
                                //.Where(x => CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotTo) >= DateTime.UtcNow)
                                .ToList();

                    // Get's No of Rows Count   
                    int TotalCount = source.Count();
                    // Display TotalCount to Records to User  
                    return TotalCount;
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int TotalPastAppointments(string userId)
        {
            try
            {
                //Use Date time format "dd-MM-yyyy"  for dev environment
                if (_context != null)
                {
                    var source = _context.Appointments
                                 .Join(_context.DoctorTimeSlots, ap => ap.TimeSlotId, dts => dts.Id, (ap, dts) => new { ap, dts })
                                 .Join(_context.ApplicationUser, app => app.ap.PatientId, au => au.Id, (app, au) => new { app, au })
                                 .Where(x => x.app.ap.DoctorId == userId)
                                 .AsEnumerable()
                                 .Where(x => (x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Completed ||
                                             x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Cancelled ||
                                             x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Rejected)
                                                                    ||
                                         ((x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Pending
                                          || x.app.ap.Status == (int)GlobalVariables.AppointmentStatus.Confirmed)
                                          && (Convert.ToDateTime(x.app.dts.Date.ToString(@"MM-dd-yyyy") + " " +
                                                                x.app.dts.SlotTo.ToString(@"hh\:mm\:ss")
                                                               ) < DateTime.UtcNow)
                                      //&& (CommonFunctions.ConvertToDateTime2(x.app.dts.Date, x.app.dts.SlotTo) < DateTime.UtcNow))
                                      ))
                                 .ToList();

                    int totalCount = source.Count();
                    return totalCount;
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckForSameDateSlot(string patientID, DateTime date, TimeSpan SlotFrom)
        {
            try
            {
                var _allsameDateTimeSlotsList = _context.DoctorTimeSlots.Where(x => x.Date == date && x.SlotFrom == SlotFrom).ToList();
                foreach (var slotdata in _allsameDateTimeSlotsList)
                {
                    var _isBooked = _context.Appointments.Where(x => x.TimeSlotId == slotdata.Id && x.PatientId == patientID && x.IsDeleted == false).FirstOrDefault();
                    if (_isBooked != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }


        }


    }
}
