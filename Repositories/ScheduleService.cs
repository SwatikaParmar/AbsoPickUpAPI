using AnfasAPI.Data;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime;
using AnfasAPI.IServices;
using System.Globalization;
using System.Collections;
using AnfasAPI.Common;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.EntityFrameworkCore;

namespace AnfasAPI.Repositories
{
    public class ScheduleService : IScheduleService
    {
        private ApplicationDbContext _context;
        public IConfiguration _configuration { get; }
        public ScheduleService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
        public SqlConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        #region InsertDoctorSchedule
        public int InsertDoctorSchedule(DocScheduleVM model)
        {
            try
            {
                if (model.schedule.Length > 0)
                {
                    foreach (var shift in model.schedule)
                    {

                        var datetime = CommonFunctions.SplitStringToParts(shift.TimeShift.FromTime);
                        var date = datetime[0];
                        var fromTime = datetime[1];
                        var toTime = CommonFunctions.SplitStringToParts(shift.TimeShift.ToTime)[1];
                        DoctorSchedule doctorSchedulemodel = new DoctorSchedule();
                        doctorSchedulemodel.UserId = model.UserId;
                        doctorSchedulemodel.IsAvailable = model.IsAvailable;
                        doctorSchedulemodel.Date = DateTime.ParseExact(date, GlobalVariables.DefaultDateFormat, null);
                        doctorSchedulemodel.fromTime = TimeSpan.Parse(fromTime);
                        doctorSchedulemodel.toTime = TimeSpan.Parse(toTime);
                        doctorSchedulemodel.CreatedDate = DateTime.UtcNow;
                        bool ifExists = IsShiftAlreadyExists(model.UserId, doctorSchedulemodel.Date, doctorSchedulemodel.fromTime, doctorSchedulemodel.toTime);
                        if (ifExists)
                        {
                            return 0;
                        }
                        else
                        {
                            // Insert TimeShift in DoctorSchedule 
                            bool IsShiftAdded = SaveDoctorSchedule(doctorSchedulemodel);
                            if (IsShiftAdded)
                            {
                                foreach (var slots in shift.TimeSlots)
                                {
                                    var slotstartTime = CommonFunctions.SplitStringToParts(slots.SlotStart)[1];
                                    var slotendtime = CommonFunctions.SplitStringToParts(slots.SlotEnd)[1];
                                    DoctorTimeSlots doctorTimeSlots = new DoctorTimeSlots();
                                    doctorTimeSlots.UserId = model.UserId;
                                    doctorTimeSlots.Date = DateTime.ParseExact(date, GlobalVariables.DefaultDateFormat, null);
                                    doctorTimeSlots.SlotFrom = TimeSpan.Parse(slotstartTime);
                                    doctorTimeSlots.SlotTo = TimeSpan.Parse(slotendtime);
                                    doctorTimeSlots.IsSlotAvailable = true;
                                    doctorTimeSlots.IsLocked = false;
                                    doctorTimeSlots.CreatedDate = DateTime.UtcNow;
                                    // Insert TimeSlots in DoctorTimeSlots
                                    bool isTimeSlotAdded = SaveDoctorTimeSlots(doctorTimeSlots);
                                } 
                            }
                            else
                            {
                                return -2;
                            } 
                        } 
                    }
                    return 1;
                }
                else
                {
                    return -1;
                } 
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SaveDoctorSchedule(DoctorSchedule doctorSchedulemodel)
        {
            try
            {
                _context.DoctorSchedule.Add(doctorSchedulemodel);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool SaveDoctorTimeSlots(DoctorTimeSlots doctorTimeSlots)
        {
            try
            {
                _context.DoctorTimeSlots.Add(doctorTimeSlots);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region "GetAvailableDates"
        public List<AvailableDatesViewModel> GetAvailableDates(string UserId, string Date)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(Date, GlobalVariables.DefaultDateFormat, null);
                int month = dt.Month;
                int year = dt.Year;
                // DateTime lastDateofMonth = new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);

                // Here we can show total 4 months data to overcome UTC issue. we use between cases from two dates
                // i.e lastMonth data, current Month data & + next 2 months data

                // to get previous Month first date 
                //DateTime startDate = new DateTime(dt.Year, dt.Month, 1).AddMonths(-1);
                DateTime startDate = new DateTime(dt.Year, dt.Month, 1).AddDays(-5);

                // to get next to next Month last date from current month 
                DateTime endDate = new DateTime(dt.Year, dt.Month, 1).AddMonths(3).AddDays(-1);

                var scheduledDate = _context.DoctorSchedule.Where(x => x.UserId == UserId && x.IsAvailable == true && x.Date >= startDate && x.Date <= endDate &&  x.IsDeleted == false).Select(
                                                                              x => new AvailableDatesViewModel
                                                                              {
                                                                                  AvailableDates = x.Date.ToString(GlobalVariables.DefaultDateFormat),
                                                                                  FromTime = x.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.fromTime),
                                                                                  ToTime = x.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.toTime)
                                                                              }).Distinct().ToList();

                
                return scheduledDate;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region "GetAvailableTimeShift"
        public List<ShiftViewModel> GetAvailableTimeShift(string UserId, string Date)
        {
            DateTime dt = DateTime.ParseExact(Date, GlobalVariables.DefaultDateFormat, null);
            int month = dt.Month;
            int year = dt.Year;
            // Here we can show total 3 dates data to overcome UTC issue. we use between cases from two dates
            // i.e previous date data, passed date data & + next date data

            // to get previous date 
            DateTime startDate = dt.AddDays(-1);

            // to get next date from passed date 
            DateTime endDate = dt.AddDays(1);
            var _timeShift = _context.DoctorSchedule.Where(x => x.UserId == UserId && x.Date >= startDate && x.Date <= endDate && x.IsDeleted == false)
                                                    .Select(
                                                                x => new ShiftViewModel
                                                                {
                                                                    Id = x.Id,
                                                                    FromTime = x.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.fromTime),
                                                                    ToTime = x.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.toTime)
                                                                }).ToList();

            var _orderedTimeShift = _timeShift.OrderBy(s => s.FromTime).ToList();

            return _orderedTimeShift;
        }
        #endregion

        #region "GetDoctorTimeSlotByDate"
        public List<DoctorTimeSlotsViewModel> GetDoctorTimeSlotByDate(string userId, string selectedDate)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(selectedDate, GlobalVariables.DefaultDateFormat, null);
                int month = dt.Month;
                int year = dt.Year;

                // Here we can show total 3 dates data to overcome UTC issue. we use between cases from two dates
                // i.e previous date data, passed date data & + next date data

                // to get previous date 
                DateTime startDate = dt.AddDays(-1);

                // to get next date from passed date 
                DateTime endDate = dt.AddDays(1);
                var _timeSlotsList = _context.DoctorTimeSlots.Where(x => x.UserId == userId && x.Date >= startDate && x.Date <= endDate && x.IsDeleted == false).Select(
                                                                                                                    x => new DoctorTimeSlotsViewModel
                                                                                                                    {
                                                                                                                        TimeSlotId = x.Id,
                                                                                                                        Date = x.Date.ToString(GlobalVariables.DefaultDateFormat),
                                                                                                                        SlotFrom = x.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.SlotFrom),
                                                                                                                        SlotTo = x.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + CommonFunctions.ConvertTimeSpanToString(x.SlotTo),
                                                                                                                        IsSlotAvailable = x.IsSlotAvailable,
                                                                                                                       
                                                                                                                    }).ToList();
                var _orderedTimeShift = _timeSlotsList.OrderBy(s => s.SlotFrom).ToList();

                return _orderedTimeShift;
                
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region "UpdateDoctorSchedule"
        //public bool UpdateDoctorSchedule(UpdateScheduleViewModel model, string userId)
        //{
        //    try
        //    {
                
        //        var _shiftTime = _context.DoctorSchedule.Where(x => x.Id == model.Id && x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
        //        if (_shiftTime != null)
        //        {
        //            DateTime passedDate = _shiftTime.Date;
        //            TimeSpan slotFrom = _shiftTime.fromTime;
        //            TimeSpan slotTo = _shiftTime.toTime;
        //            var _slotsList = _context.DoctorTimeSlots.Where(x => x.Date == passedDate && x.UserId == userId && x.SlotFrom >= slotFrom && x.SlotTo <= slotTo && x.IsDeleted == false).ToList();
        //            var _confirmedSlots = _context.Appointments.Where(x => x.DoctorId == userId && (x.Status == (int)GlobalVariables.AppointmentStatus.Confirmed || x.Status == (int)GlobalVariables.AppointmentStatus.Pending)).ToList();

        //            var _existingSlots = _slotsList.Where(a => _confirmedSlots.Any(b => b.TimeSlotId == a.Id)).FirstOrDefault();
        //            if (_existingSlots != null)
        //            {
        //                return false;   // shift can not be edited
        //            }
        //            else
        //            {
        //                // UpdateDoctorSchedule
        //                var fromTime = CommonFunctions.SplitStringToParts(model.schedule.TimeShift.FromTime)[1];
        //                var toTime = CommonFunctions.SplitStringToParts(model.schedule.TimeShift.ToTime)[1];
        //                _shiftTime.fromTime = TimeSpan.Parse(fromTime);
        //                _shiftTime.toTime = TimeSpan.Parse(toTime);
        //                _shiftTime.UpdatedDate = DateTime.UtcNow;
        //                _context.DoctorSchedule.Update(_shiftTime);
        //                _context.SaveChanges();
                       

        //                // UpdateDoctorTimeSlots
        //                foreach (var item in _slotsList)
        //                {
        //                    var slotData = _context.DoctorTimeSlots.Where(x => x.Id == item.Id).FirstOrDefault();

        //                    if (slotData != null)
        //                    {
        //                        slotData.IsDeleted = true;
        //                        slotData.DeletedBy = userId;
        //                        slotData.DeletedDate = DateTime.UtcNow;
        //                        _context.DoctorTimeSlots.Update(slotData);
                           
        //                    }
        //                }
        //                _context.SaveChanges();

        //                // inserting updated time slots
        //                foreach (var slots in model.schedule.TimeSlots)
        //                {
        //                    var datetime = CommonFunctions.SplitStringToParts(slots.SlotStart);
        //                    var date = datetime[0];
        //                    var slotstartTime = datetime[1];
        //                    var slotendtime = CommonFunctions.SplitStringToParts(slots.SlotEnd)[1];
        //                    DoctorTimeSlots doctorTimeSlots = new DoctorTimeSlots();
        //                    doctorTimeSlots.UserId = userId;
        //                    doctorTimeSlots.Date = DateTime.ParseExact(date, GlobalVariables.DefaultDateFormat, null);
        //                    doctorTimeSlots.SlotFrom = TimeSpan.Parse(slotstartTime);
        //                    doctorTimeSlots.SlotTo = TimeSpan.Parse(slotendtime);
        //                    doctorTimeSlots.IsSlotAvailable = true;
        //                    doctorTimeSlots.IsLocked = false;
        //                    doctorTimeSlots.CreatedDate = DateTime.UtcNow;
        //                    // Insert TimeSlots in DoctorTimeSlots
        //                    bool isTimeSlotAdded = SaveDoctorTimeSlots(doctorTimeSlots);
        //                }
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //--------------------New Method Start---------------------
        public bool UpdateDoctorSchedule(UpdateScheduleViewModel model, string userId)
        {
            try
            {

                var _shiftTime = _context.DoctorSchedule.Where(x => x.Id == model.Id && x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
                if (_shiftTime != null)
                {
                    DateTime passedDate = _shiftTime.Date;
                    TimeSpan slotFrom = _shiftTime.fromTime;
                    TimeSpan slotTo = _shiftTime.toTime;
                    var _slotsList = _context.DoctorTimeSlots.Where(x => x.Date == passedDate && x.UserId == userId && x.SlotFrom >= slotFrom && x.SlotTo <= slotTo && x.IsDeleted == false).ToList();
                    var _confirmedSlots = _context.Appointments.Where(x => x.DoctorId == userId && (x.Status == (int)GlobalVariables.AppointmentStatus.Confirmed || x.Status == (int)GlobalVariables.AppointmentStatus.Pending)).ToList();

                    var _existingSlots = _slotsList.Where(a => _confirmedSlots.Any(b => b.TimeSlotId == a.Id)).FirstOrDefault();
                    if (_existingSlots != null)
                    {
                        return false;   // shift can not be edited
                    }
                    else
                    {
                        // UpdateDoctorSchedule
                        var modeldate = CommonFunctions.SplitStringToParts(model.schedule.TimeShift.FromTime)[0];
                        var mdate =  DateTime.ParseExact(modeldate, GlobalVariables.DefaultDateFormat, null);
                        if (passedDate != mdate)
                        {
                            _shiftTime.Date = mdate;
                        }
                        var fromTime = CommonFunctions.SplitStringToParts(model.schedule.TimeShift.FromTime)[1];
                        var toTime = CommonFunctions.SplitStringToParts(model.schedule.TimeShift.ToTime)[1];
                        _shiftTime.fromTime = TimeSpan.Parse(fromTime);
                        _shiftTime.toTime = TimeSpan.Parse(toTime);
                        _shiftTime.UpdatedDate = DateTime.UtcNow;
                        _context.DoctorSchedule.Update(_shiftTime);
                        _context.SaveChanges();


                        // UpdateDoctorTimeSlots
                        foreach (var item in _slotsList)
                        {
                            var slotData = _context.DoctorTimeSlots.Where(x => x.Id == item.Id).FirstOrDefault();

                            if (slotData != null)
                            {
                                slotData.IsDeleted = true;
                                slotData.DeletedBy = userId;
                                slotData.DeletedDate = DateTime.UtcNow;
                                _context.DoctorTimeSlots.Update(slotData);

                            }
                        }
                        _context.SaveChanges();

                        // inserting updated time slots
                        foreach (var slots in model.schedule.TimeSlots)
                        {
                            var datetime = CommonFunctions.SplitStringToParts(slots.SlotStart);
                            var date = datetime[0];
                            var slotstartTime = datetime[1];
                            var slotendtime = CommonFunctions.SplitStringToParts(slots.SlotEnd)[1];
                            DoctorTimeSlots doctorTimeSlots = new DoctorTimeSlots();
                            doctorTimeSlots.UserId = userId;
                            doctorTimeSlots.Date = DateTime.ParseExact(date, GlobalVariables.DefaultDateFormat, null);
                            doctorTimeSlots.SlotFrom = TimeSpan.Parse(slotstartTime);
                            doctorTimeSlots.SlotTo = TimeSpan.Parse(slotendtime);
                            doctorTimeSlots.IsSlotAvailable = true;
                            doctorTimeSlots.IsLocked = false;
                            doctorTimeSlots.CreatedDate = DateTime.UtcNow;
                            // Insert TimeSlots in DoctorTimeSlots
                            bool isTimeSlotAdded = SaveDoctorTimeSlots(doctorTimeSlots);
                        }
                        return true;
                    }
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
        //--------------------New Method End ----------------------
        #endregion

        #region "DeleteSchedule"
        public bool DeleteSchedule(int Id, string userId)
        {
            try
            {
                var _timeShift = _context.DoctorSchedule.Where(x => x.Id == Id && x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
                DateTime passedDate = _timeShift.Date;
                TimeSpan slotFrom = _timeShift.fromTime;
                TimeSpan slotTo = _timeShift.toTime;
                var _slotsList = _context.DoctorTimeSlots.Where(x => x.Date == passedDate && x.UserId == userId && x.SlotFrom >= slotFrom && x.SlotTo <= slotTo && x.IsDeleted == false).ToList();
                var _confirmedSlots = _context.Appointments.Where(x => x.DoctorId == userId && (x.Status == (int)GlobalVariables.AppointmentStatus.Confirmed || x.Status == (int)GlobalVariables.AppointmentStatus.Pending)).ToList();

                var _existingSlots = _slotsList.Where(a => _confirmedSlots.Any(b => b.TimeSlotId == a.Id)).FirstOrDefault();
                if (_existingSlots != null)
                {
                    return false;
                }
                else
                {
                     // DeleteTimeShift
                    _timeShift.IsDeleted = true;
                    _timeShift.DeletedBy = userId;
                    _timeShift.DeletedDate = DateTime.UtcNow;
                    _context.DoctorSchedule.Update(_timeShift);
                    _context.SaveChanges();
                    // DeleteTimeSlot
                    foreach (var item in _slotsList)
                    {
                        var slotData = _context.DoctorTimeSlots.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (slotData != null)
                        {
                            slotData.IsDeleted = true;
                            slotData.DeletedBy = userId;
                            slotData.DeletedDate = DateTime.UtcNow;
                            _context.DoctorTimeSlots.Update(slotData);

                        }
                    }
                    _context.SaveChanges();
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region "UpdateTimeSlotStatus"
        public async Task<bool> UpdateTimeSlotStatus(long timeSlotId, bool status)
        {
            try
            {
                var slotData = await _context.DoctorTimeSlots.Where(x => x.Id == timeSlotId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (slotData != null)
                {
                    slotData.IsSlotAvailable = status;
                    _context.Update(slotData);
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
        #endregion

        #region "Private Methods"
        private bool IsShiftAlreadyExists(string doctorId, DateTime date, TimeSpan fromTime, TimeSpan toTime)
        {
            try
            {
                if (_context.DoctorSchedule.Any(x => x.UserId == doctorId && x.Date == date &&
                x.fromTime <= fromTime && x.toTime >= toTime && x.IsDeleted == false))
                {
                    return true;
                }
                else if(_context.DoctorSchedule.Any(x => x.UserId == doctorId && x.Date == date &&
                x.fromTime >= fromTime && x.toTime <= toTime && x.IsDeleted == false))
                {
                    return true;
                }
                else if(_context.DoctorSchedule.Any(x => x.UserId == doctorId && x.Date == date &&
                x.fromTime > fromTime && x.toTime < toTime && x.IsDeleted == false))
                {
                    return true;
                }
                else if (_context.DoctorSchedule.Any(x => x.UserId == doctorId && x.Date == date  && x.IsDeleted == false &&
                 ((fromTime > x.fromTime && fromTime < x.toTime) || (toTime > x.fromTime && toTime < x.toTime))))
                {
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

      
        #endregion

    }
}
