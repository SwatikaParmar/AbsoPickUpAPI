using AnfasAPI.IServices;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.JobScheduler.Jobs
{
    public class NotificationCallReminderJob : IJob
    {
        private INotificationService _notificationService;
        public NotificationCallReminderJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task Execute(IJobExecutionContext context)
       {
            try
            {
                // call reminder code to be executed
                await _notificationService.SendPushNotificationForCallReminder();

            }
            catch(Exception)
            {
                throw;
            }
            await Task.CompletedTask;
        }
    }
}
