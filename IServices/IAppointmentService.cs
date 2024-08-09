using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IAppointmentService
    {
        Task<ResponseBookAppointmentViewModel> BookAppointment(Appointments model);
        Task<int> UpdateAppointmentStatus(string userId, string userRole, AppointmentStatusChangeViewModel model);
        bool DeleteAppointment(string appointmentId, string userId, int appointmentStatus);
        // Patient Upcoming Appointment
        List<PatientUpcomingAppointmentViewModel> GetPatientUpcomingAppointments(string userId, int appointmentStatus);
        FilterationResponseModel<PatientPastAppointmentViewModel> GetPatientPastAppointments(FilterationListViewModel model, string userId);

        // Doctor Upcoming Appointments (Pending or Confirmed)
        FilterationResponseModel<DoctorUpcomingAppointmentsViewModel> GetDoctorUpcomingAppointments(FilterationListViewModel model, string userId, int appointmentStatus);

        // Doctor Past Appointments (Cancelled, Rejected or Completed)
        FilterationResponseModel<DoctorPastAppointmentsViewModel> GetDoctorPastAppointments(FilterationListViewModel model, string userId);

        AppointmentDetailsViewModel GetAppointmentById(string id);

        int TotalUpcomingAppointments(string userId);
        int TotalPendingAppointments(string userId);
        int TotalPastAppointments(string userId);
    }
}
