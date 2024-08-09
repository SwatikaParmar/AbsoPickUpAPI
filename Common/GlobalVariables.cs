using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Common
{
    public static class GlobalVariables
    {
        public static readonly string DefaultDateFormat = "dd-MM-yyyy";
        public static readonly string ServerDateFormat = "MM-dd-yyyy";
        public static readonly int SlotDurationTime = 30;
        public static readonly int preBookingTimeInMinutes = 15;
        public static readonly int preBookingConfirmedTimeInMinutes = 15;
        public static readonly int preBookingCancelTimeInMinutes = 15;

        #region " Google Firebase region"
        public static readonly string FirebaseServerKeyForDoctor = "AAAAq-bfNLo:APA91bHgp8P2vVbATGqG9LMKHSRW0RnfZx09uV6-6guKF5pRKkWjo-S8ot4Hz-vdYjudm4HNwLMEoLciFAeOWI671nRUNmfl-ylhRyQx8WRqlzij8mD_TjpV8MLCnqFYFtIYC4s1c8BS";
        public static readonly string FirebaseDoctorSenderID = "738312795322";
        public static readonly string FirebaseServerKeyForPatient = "AAAAPOg2cJw:APA91bEgxZFf0_cr7k6r0OHUjjB57yqg_CXKmIrVeWlLTojPt2HW_Cm_kYiuvvj1_vZ9pq6M5kiuyhVMQPLA_nrily3z28jdG9uMaB3vwt7oaI2zDSyPvn-ooJndhDTdbRpvHyZR35Jq";
        public static readonly string FirebasePatientSenderID = "261593919644";
        //public static readonly string FirebaseServerKey = "AAAAfKTEYAY:APA91bGQGnw1muPBhKIDoTnUSFn8Lh2Mn66F47FkvZefp5JpqUMu1rUPzWI6Ekp7O-z4bcJZdiY1NW7eF7DpNNXBmch4zsn4EQ-OfqFl5VSzta85Y4br3JAX8onJvm-OEl9kCImpwxR5";
        public static readonly Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");

        #endregion
        public enum PatientStatus
        {
            Active = 1,
            InActive,
        }

        #region " Folder Path i.e Images Container Names"
        public static readonly string docsContainer = "DoctorDocuments";
        public static readonly string profilePictureContainer = "UserImages";
        public static readonly string ServicePictureContainer = "ServiceImages";
        public static readonly string DoctorProfilePicContainer = "DoctorImages";
        public static readonly string FounderPictureContainer = "FounderImage";
        public static readonly string medicaldocsContainer = "PatientMedicalReports";
        public static readonly string blogImagesContainer = "BlogImages";
        public static readonly string bannerImagesContainer = "BannerImages";
        public static readonly string specialityImagesContainer = "SpecialityImages";
        public static readonly string ChatImagesContainer = "ChatImages";
        public static readonly string ChatDocumentContainer = "ChatDocuments";
        public static readonly string BookedServiceReportContainer = "BookedServiceReport";
        #endregion
        public static readonly string ImageUrl = "https://elasticbeanstalk-us-east-2-369771416311.s3.us-east-2.amazonaws.com/FilesToSave/";
       
        #region " Opentok Media Manager keys(Video call streaming)"
        public static readonly int OpenTokApiKey = 47469721;
        public static readonly string OpenTokApiSecret = "3424bc8f76c8a356d02cd99619228c08dfa084b4";
        #endregion

        #region " Google Token Validate Url"
        public const string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}";
        #endregion

        //#region " Twillio Validate Credentials"
        //public const string twilio_accountSid = "ACc2f664aa6d5845bda5d532231bd936af";
        //public const string twilio_authToken = "7787e3d9733a9b1456a86c184ad33e9c";
        //public const string twilio_verificationSid = "VAb4e902231822ee0c5465e51d683efc3f";
        //public const string twilio_phoneNumber = "8283731859";
        //#endregion
        #region "Twillio Validate Credentials"
        public const string twilio_accountSid = "AC97fc77fef265c63badadeb643d4b894d";
        public const string twilio_authToken = "cd94c45494dc87776fc0418ab2795287";
        public const string twilio_verificationSid = "VA530e71f197fe31604660535d12f17a4b";
        public const string twilio_phoneNumber = "8544815315";
        #endregion
        public enum AppointmentStatus
        {
            Pending = 0, // 0. Pending appointments
            Confirmed, // 1. Confirmed or Accepted appointments
            Rejected, // 2. Deny or Rejected
            Cancelled, // 3. Confirmed appointment cancelled
            Reminder, //4. For Notification
            Completed //5. Completed
        }

        public enum ServiceStatus
        {
            Pending = 0, // 0. Pending appointments
            Confirmed, // 1. Confirmed or Accepted appointments
            Rejected, // 2. Deny or Rejected
            Cancelled, // 3. Confirmed appointment cancelled
            Completed //5. Completed
        }

        public enum BlogStatus
        {
            Pending = 0, // 0. Pending 
            Approved, // 1. Approved
            Rejected, // 2. Rejected
        }

        public enum ServiceType
        {
            HomeService = 0, // 0. Pending appointments
            LabService, // 1. Confirmed or Accepted appointments
            Teleconsultancy, // 2. Deny or Rejected
        }
        public enum AnfasServiceType
        {
            HomeService = 0, // 0. Home Service
            LabService, // 1. Lab Service
            RadiologyService, // 2. Radiology Service
            DialysisService, // 2. Radiology Service
            RehabilationService, // 2. Radiology Service
        }

        public enum DeviceTypes
        {
            android = 1,
            ios,
            web
        }
        public enum UserRole
        {
            Staff,
            Guest,
            SuperAdmin,
            Hospital,
            Patient,
            Doctor,
            Laboratory
        }
        public enum Gender
        {
            Male = 1,
            Female,
            Others
        }
        public enum DoctorApplicationStatus
        {
            Incomplete = 0,
            Pending,
            Accepted,
            Rejected
        }
        public enum PaymentStatus
        {
            Pending = 0, //your payment has not yet been sent to the bank.
            Success, //your checking, savings or other bank account payment has been processed and accepted
            Complete, //your checking, savings or other bank account payment has been processed and accepted
            Cancelled, //you stopped the payment before it was processed
            Rejected //your payment was not accepted when it was processed by the bank or credit card company.
        }

        public enum TransactionType
        {
            Credit = 1,
            Debit,
            Refund
        }
        public enum LoginType
        {
            Email,
            Facebook,
            Google
        }
        public enum LastScreenInfo
        {
            PersonalInfo = 1,
            Verification,
            ProfessionalInfo,
            BankInfo
        }
        public enum TwilioChannelTypes
        {
            Sms = 1,
            Call,
            Email
        }

        public enum PurposeTypes
        {
            Appointment = 1,
            Message,
            Reminder,
            Payment,
            Prescription
        }
        public enum NotificationTypes
        {
            NEW_APPOINTMENT_REQUEST = 1,
            CANCELLED_APPOINTMENT,
            CONFIRMED_APPOINTMENT,
            REJECTED_APPOINTMENT,
            MESSAGE_RECIEVED,
            ADMIN_MESSAGES,
            ADMIN_APPROVED,
            ADMIN_REJECTED,
            PATIENT_WAITING,
            CALL_REMIND_ALERT,
            CALL_REMAINING_TIME_ALERT,
            DOCTOR_PRESCRIPTION,
            PENDING_APPOINTMENTS_REMINDER
        }
        public enum ReportStatus
        {
            Pending = 0, // 0. Pending
            Uploaded, // 
        }


        #region "Enum Related Methods"
        public static string GetDoctorApplicationStatus(int enumId)
        {
            if (enumId == 1)
            {
                return DoctorApplicationStatus.Pending.ToString();
            }
            else if (enumId == 2)
            {
                return DoctorApplicationStatus.Accepted.ToString();
            }
            else if (enumId == 3)
            {
                return DoctorApplicationStatus.Rejected.ToString();
            }
            else
            {
                return DoctorApplicationStatus.Incomplete.ToString();
            }
        }

        public static string GetAppointmentStatusNames(int enumId)
        {
            if (enumId == 1)
            {
                return AppointmentStatus.Confirmed.ToString();
            }
            else if (enumId == 2)
            {
                return AppointmentStatus.Rejected.ToString();
            }
            else if (enumId == 3)
            {
                return AppointmentStatus.Cancelled.ToString();
            }
            else if (enumId == 4)
            {
                return AppointmentStatus.Reminder.ToString();
            }
            else if (enumId == 5)
            {
                return AppointmentStatus.Completed.ToString();
            }
            else
            {
                return AppointmentStatus.Pending.ToString();
            }
        }
        #endregion

        public enum RequestLeaveStatus
        {
            Pending = 0, // 0. Pending appointments
            Approved, // 1. Confirmed or Accepted appointments
            Rejected, // 2. Deny or Rejected
        }
        public static string GetRequestedLeaveStatusNames(int enumId)
        {
            if (enumId == 1)
            {
                return RequestLeaveStatus.Approved.ToString();
            }
            else if (enumId == 2)
            {
                return RequestLeaveStatus.Rejected.ToString();
            }
            else
            {
                return RequestLeaveStatus.Pending.ToString();
            }
        }
    }
}
