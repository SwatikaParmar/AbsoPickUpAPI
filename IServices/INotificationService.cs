using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface INotificationService
    {
        bool CreateNotification(Notifications model);
        Task<bool> CreateNotificationOnAppointmentStatusChange(Appointments appointment, string userId, string userRole);
        Task<bool> CreatePushNotificationOnAppointmentStatusChange(Appointments appointment, string userRole, DoctorTimeSlotsViewModel _timeSlotsModel);
        FilterationResponseModel<PatientNotificationListViewModel> GetNotificationListByUser(FilterationListViewModel model, string userId);
        FilterationResponseModel<NotificationListViewModel> GetNotificationListForDoctor(FilterationListViewModel model, string userId);
        bool MarkNotificationAsRead(string notificationId, string userId);
        bool DeleteNotification(string NotificationId);
        bool DeleteAllNotifications(string userId);
        Task<bool> SendPushNotification(string[] deviceTokens, string title ,string body);
        Task<bool> SendPushNotificationsForDoctor(string[] deviceTokens, string title, string body, object data);
        Task<bool> SendPushNotificationsForPatient(string[] deviceTokens, string title, string body, object data);
        Task<bool> SendEmailOnAppointmentStatusChange(Appointments appointment, string userRole, DoctorTimeSlotsViewModel _timeSlotsModel);

        Task SendPushNotificationForCallReminder();

    }
}
