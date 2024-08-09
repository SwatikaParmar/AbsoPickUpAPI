using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IScheduleService
    {
        int InsertDoctorSchedule(DocScheduleVM model);
        List<AvailableDatesViewModel> GetAvailableDates(string UserId, string Date);
        List<DoctorTimeSlotsViewModel> GetDoctorTimeSlotByDate(string userId, string selectedDate);
        Task<bool> UpdateTimeSlotStatus(long timeSlotId, bool status);
        List<ShiftViewModel> GetAvailableTimeShift(string UserId, string Date);
        bool DeleteSchedule(int Id, string userId);
        bool UpdateDoctorSchedule(UpdateScheduleViewModel model, string userId);
        
        
    }
}
