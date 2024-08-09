using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Common
{
    public class EmailMessages
    {
        public static readonly string regardsMsgFromTeam = "Anfas";
        public static readonly string resetPasswordSubject = "Reset Password";
        public static readonly string confirmationEmailSubject = "Confirmation Email";
        public static readonly string adminApprovedDoctorEmailSubject = "Greetings..!! Your Account is Approved.";
        public static readonly string adminRejectedDoctorEmailSubject = "Sorry, Your Account has been rejected";
        //Appointment 
        public static readonly string newAppointmentRequest = "New appointment from";
        public static readonly string appointmentConfirmed = "your appointment is confirmed by Dr.";
        public static readonly string appointmentRejected = "your appointment is rejected by";
        public static readonly string appointmentCancelled = "your appointment is cancelled by";

        //Prescription
        public static readonly string prescriptionArrived = "your prescription from doctor";

        #region Email messages for Auth Controller

        public static string GetUserRegistrationEmailConfirmationMsg(int Otp)
        {
            string msg = $"Hi, <br/><br/> Your confirmation code to access your account is {Otp}. <br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }

        public static string GetUserRegistrationSendPasswordMsg(string password)
        {
            string msg = $"Hi, <br/><br/> Your password to access your account is {password}. <br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetUserForgotPasswordMsg(string Name, int Otp)
        {
            string msg = $"Hi {Name}, <br/><br/> Your one time password is {Otp} for reseting password on Anfas. <br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetUserRegistrationResendEmailConfirmationMsg(string Name, int Otp)
        {
            string msg = $"Hi {Name}, <br/><br/> Your new confirmation code to access your account is {Otp}. <br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetUserChangeEmailMsg(string Name, int Otp)
        {
            string msg = $"Hi {Name}, <br/><br/> Your one time password is {Otp} for changing email on Anfas. <br/><br/> Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }

        public static string GetDoctorApprovedByAdminMsg(string DoctorName)
        {
            string msg = $"Hi Dr.{DoctorName}, <br/><br/> Greetings!! <br/><br/>Your Account is Approved by Our Team. Now you can able to set schedule and handle patients on video call & provide best services to clients.<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }

        public static string GetDoctorRejectedByAdminMsg(string DoctorName)
        {
            string msg = $"Hi Dr.{DoctorName}, <br/><br/> Greetings!! <br/><br/> Sorry , Your Account has been rejected by admin.<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        #endregion

        #region Email messages for Appointment Controller
        public static string GetCreateAppointmentDoctorEmailNotificationMsg(string doctorName, string patientName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = $"Hi Dr.{doctorName}," + "<br/><br/>" + " You have a new appointment scheduled with " + patientName + " at" + " " + SlotFrom + " " + "to " + SlotTo + " " + "on " + AppointmentDate + " " +
                " Please confirm your availability in the Doctors' App." + "<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }


        //Email Subject
        public static string GetNewAppointmentEmailSubject(string fromUserName)
        {
            string msg = newAppointmentRequest + " " + fromUserName + ".";
            return msg;
        }
        public static string GetAppointmentConfirmedEmailSubject(string fromUserName)
        {
            string msg = appointmentConfirmed + " " + fromUserName + ".";
            return msg;
        }
        public static string GetAppointmentConfirmedEmailMsg(string doctorName, string patientName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = $"Hi {patientName}, <br/><br/>" + appointmentConfirmed + " " + doctorName + " " + "at " + " " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + "." +
                " If you have any question, please feel free to chat with him." + "<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetAppointmentRejectedByDoctorEmailMsg(string doctorName, string patientName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = $"Hi {patientName}, <br/><br/>" + appointmentRejected + " " + "Dr. " + doctorName + " at " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". " + "<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetAppointmentRejectedByDoctorEmailSubject(string fromUserName)
        {
            string msg = appointmentRejected + " " + "Dr. " + fromUserName + ".";
            return msg;
        }
        public static string GetAppointmentCancelledByDoctorEmailMsg(string doctorName, string patientName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = $"Hi {patientName}, <br/><br/>" + appointmentCancelled + " " + "Dr. " + doctorName + " at " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". " + "<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetAppointmentCancelledByDoctorEmailSubject(string fromUserName)
        {
            string msg = appointmentCancelled + " " + "Dr. " + fromUserName + ".";
            return msg;
        }
        public static string GetAppointmentCancelledByPatientEmailNotificationMsg(string doctorName, string patientName, string SlotFrom, string SlotTo, string AppointmentDate)
        {
            string msg = $"Hi Dr.{doctorName}, <br/><br/>" + appointmentCancelled + " " + patientName + " at " + SlotFrom + " " + "to " + SlotTo + " on " + AppointmentDate + ". " + "<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetAppointmentCancelledByPatientEmailSubject(string fromUserName)
        {
            string msg = appointmentCancelled + " " + fromUserName + ".";
            return msg;
        }

        #endregion

        #region Email messages for Prescription Controller
        public static string GetPatientPrescriptionEmailNotificationMsg(string _patientName, string _doctorName, string _appointmentDate)
        {
            string msg = $"Hi {_patientName}, < br />< br /> Please check your prescription sent by Dr. {_doctorName} for {_appointmentDate}"
                 + "<br/><br/>Thanks <br/>" + regardsMsgFromTeam;
            return msg;
        }
        public static string GetPatientPrescriptionEmailSubject(string fromUserName)
        {
            string msg = prescriptionArrived + " " + fromUserName + " " + "has been sent";
            return msg;
        }
        #endregion

    }
}
