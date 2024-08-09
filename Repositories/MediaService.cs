using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class MediaService : IMediaService
    {
        private ApplicationDbContext _context;
        public MediaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CheckAppointmentCallSession(AppointmentDetailsViewModel appointmentData)
        {
            DateTime appointmentDate = DateTime.ParseExact(appointmentData.Date, GlobalVariables.DefaultDateFormat, null);
            DateTime appointmentStartTime = appointmentDate.Add(TimeSpan.Parse(appointmentData.SlotFrom));
            DateTime appointmentEndTime = appointmentDate.Add(TimeSpan.Parse(appointmentData.SlotTo));
            if (DateTime.UtcNow >= appointmentStartTime && DateTime.UtcNow <= appointmentEndTime)
            {
                return true;
            }
            else
            {
                return false;
            } 
        }

        public AppointmentMediaSession CheckCreateAppointmentCallSession(AppointmentMediaSession sessionModel)
        {
            var model = _context.AppointmentMediaSession.Where(x => x.AppointmentId == sessionModel.AppointmentId && x.PatientId == sessionModel.PatientId && x.DoctorId == sessionModel.DoctorId).FirstOrDefault();
            if (model == null)
            {
                _context.AppointmentMediaSession.Add(sessionModel);
                _context.SaveChanges();
                return sessionModel;
            }
            return model;
        }
    }
}
