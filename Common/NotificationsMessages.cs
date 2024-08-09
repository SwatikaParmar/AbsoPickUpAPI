using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Common
{
    public static class NotificationMessages
    {
        public static readonly string newAppointmentRequest = "You have a new appointment request from";
        public static readonly string provideBestService = "Be Ready for the best service of the clients"; 
        public static readonly string readyForAppointment = "Be Ready for the appointment at scheduled time.";
        public static readonly string sorryInformMsg = "We are sorry to inform you that";
        public static readonly string appointmentCancelled = "your appointment is cancelled by ";
        public static readonly string appointmentRejected = "your appointment is rejected by ";
        public static readonly string rescheduleMessage = "Please re-schedule your appointment.";
        public static readonly string appointmentConfirmed = "Your appointment is confirmed by";

        // Appointment
        public static string GetCreateAppointmentNotificationMsg(string fromUserName) 
        {
            string msg = newAppointmentRequest + " " + fromUserName;
            return msg;
        }

        public static string GetAppointmentConfirmedNotificationMsg(string fromUserName)
        {
            string msg = appointmentConfirmed + " " + fromUserName;
            return msg;
        }

        public static string GetAppointmentRejectedNotificationMsg(string fromUserName)
        {
            string msg = sorryInformMsg + " " + appointmentRejected + " " + fromUserName;
            return msg;
        }

        public static string GetAppointmentCancelledNotificationMsg(string fromUserName) 
        {
            string msg = sorryInformMsg + " " + appointmentCancelled + " " + fromUserName; 
            return msg;
        } 

        // Prescription
        public static string GetPrescriptionNotificationMsg()
        {
            string msg = "You have got your Prescription from Doctor";
            return msg;
        }

    }

    public static class PushNotificationMessages
    {
        public static readonly string newAppointmentRequestTitle = "New Appointment Request";
        public static readonly string newAppointmentRequest = "New appointment request from";
        public static readonly string appointmentConfirmedTitle = "Appointment Confirmed";
        public static readonly string appointmentConfirmed = "Your appointment is confirmed by Dr.";
        public static readonly string appointmentRejectedTitle = "Appointment Rejected";
        public static readonly string appointmentCancelledTitle = "Appointment Cancelled";
        public static readonly string appointmentRejected = "Your appointment is rejected by";
        public static readonly string appointmentCancelled = "Your appointment is cancelled by";
        public static readonly string doctorPrescriptionTitle = "New Doctor Prescription";
        public static readonly string newDoctorPrescription = "New Doctor Prescription from";
        public static readonly string appointmentReminderTitle = "Call Reminder";
        public static readonly string appointmentReminderRegards = "Be ready for services.";
        public static readonly string appointmentReminderPatient = "Your appointment is about to start in few mins with Dr.";
        public static readonly string appointmentReminderDoctor = "Your appointment is about to start in few mins with";
        public static string GetCreateAppointmentPushNotificationMsg(string FromUserName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string message = newAppointmentRequest + " " + FromUserName + " " + "at" + " " + SlotFrom + "(UTC)" + " " + "to " + SlotTo + "(UTC)" + " " + "on " + AppointmentDate;
            return message;
        }
        public static string GetAppointmentConfirmedPushNotificationMsg(string fromUserName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = appointmentConfirmed + " " + fromUserName + " " + "at" + " " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ".";
            return msg;
        }
        public static string GetAppointmentRejectedByDoctorPushNotificationMsg(string fromUserName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = appointmentRejected + " " + "Dr. " + fromUserName + " " + "at" + " " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". ";
            return msg;
        }
        public static string GetAppointmentCancelledByDoctorPushNotificationMsg(string fromUserName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = appointmentCancelled + " " + "Dr. " + fromUserName + " at " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". ";
            return msg;
        }
        public static string GetAppointmentCancelledByPatientPushNotificationMsg(string fromUserName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = appointmentCancelled + " " + fromUserName + " at " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". ";
            return msg;
        }

        public static string GetPatientPrescriptionByDoctorMsg(string fromUserName, string appointmentId)
        {
            string msg = newDoctorPrescription + " " + fromUserName + " " + "for " + appointmentId + ". ";
            return msg;
        }

        public static string GetAppointmentReminderToPatientPushNotificationMsg(string fromUserName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = appointmentReminderPatient + " " + fromUserName + " at " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". " + appointmentReminderRegards;
            return msg;
        }
        public static string GetAppointmentReminderToDoctorPushNotificationMsg(string fromUserName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = appointmentReminderDoctor + " " + fromUserName + " at " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". " + appointmentReminderRegards;
            return msg;
        }

    }
}
