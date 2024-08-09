using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Common
{
    public static class ResponseMessages
    {
        public static readonly string msgUserRegisterSuccess = "User registered succesfully";
        public static readonly string msgUserRoleNotAuthorized = "Requested user type is not authorized";
        public static readonly string msgSomethingWentWrong = "Something went wrong. ";
        public static readonly string msgParametersNotCorrect = "Parameters are not correct";
        public static readonly string msgCouldNotFoundAssociatedUser = "Could not found any user associated with this email";
        public static readonly string msgUserNotFound = "Could not found any user";
        public static readonly string msgUserBlockedOrDeleted = "User blocked or deleted. please contact to administrator";
        public static readonly string msgEmailNotConfirmed = "Email not confirmed. Please confirm your email to access your account";
        public static readonly string msgPhoneNotConfirmed = "Phone not confirmed. Please verify your phone to access your account";
        public static readonly string msgUserLoginSuccess = "User login successfully";
        public static readonly string msgInvalidCredentials = "Username or password is incorrect";
        public static readonly string msgEmailConfirmationSuccess = "Email confirmed successfully";
        public static readonly string msgConfirmationCodeSentSuccess = "Confirmation code sent on your email";
        public static readonly string msgInvalidOTP = "Otp is invalid";
        public static readonly string msgOTPSentSuccess = "Otp sent on your email";
        public static readonly string msgPasswordResetSuccess = "Password reset successfully";
        public static readonly string msgPasswordChangeSuccess = "Password changed successfully";
        public static readonly string msgEmailAlreadyConfirmed = "Email already confirmed";
        public static readonly string msgResetEmailOtpSendSuccess = "Reset email OTP sent on your both emails";
        public static readonly string msgNewOldOtpInvalid = "Either new or old email otp is invalid";
        public static readonly string msgEmailAlreadyUsed = "The email provided already in used. please try with other email";
        public static readonly string msgEmailResetSuccess = "Email reset successfully. Please login with new email";
        public static readonly string msgLogoutSuccess = "User logout successfully";
        public static readonly string msgDbConnectionError = "Database connection error";
        public static readonly string msgBlockOrInactiveUserNotPermitted = "Block or In-Active user can't update detials";
        public static readonly string msgUserBlockedByAdmin = "You are blocked or rejected by admin. please contact to administrator";
        public static readonly string msgUserStatusPendingForApproval = "Your account is under review. Our team will contact you soon regarding approval.";
        public static readonly string msgSessionExpired = "Your current session has expired. please login again to keep all your service working";
        public static readonly string msgTokenExpired = "Access token is expired";
        public static readonly string msgSamePasswords = "Old password and New password cannot be same";
        public static readonly string msgVerifiedUser = "You are verified successfully";
        public static readonly string msgPhoneNoVerifiedSuccess = "Your phone no verified successfully";
        public static readonly string msgOTPSentOnMobileSuccess = "Otp sent on your phone no";
        public static readonly string msgProfilePicUpdated = "Profile picture updated successfully";
        public static readonly string msgProfileforPatient = "These credentials are already been used for doctor.!";
        public static readonly string msgProfileforDoctor = "These credentials are already been used for patient!";
        public static readonly string msgProfileforSuperAdmin = "This portal is valid for only user type superadmin!";
        public static readonly string msgDoctornotApproved = "You are not approved by Admin yet";
        public static readonly string msgDoctorApplicationSubmitForApproval = "Thank you for submitting your application.Our team will contact you soon regarding approval.";
        public static readonly string msgCreationSuccess = " created successfully";
        public static readonly string msgUpdationSuccess = " updated successfully";
        public static readonly string msgFoundSuccess = " found successfully";
        public static readonly string msgShownSuccess = " shown successfully";
        public static readonly string msgGotSuccess = "Data shown successfully";
        public static readonly string msgNotFound = "Could not found any ";
        public static readonly string msgListFoundSuccess = " list shown successfully";
        public static readonly string msgDeletionSuccess = " deleted successfully";
        public static readonly string msgAlreadyExists = " already exists";
        public static readonly string msgTimeBreached = "This time has already passed. Please choose another time slot";
        public static readonly string msgPreBookingTimeBreached = "Appointment cannot be booked before 1 hour. Please choose another time slot";
        public static readonly string msgAlreadyDeleted = " already deleted so you can't updated it.";
        public static readonly string msgAdditionSuccess = " added successfully";
        public static readonly string msgEmailSentSuccess = "Email sent Successfully";
        public static readonly string msgSentSuccess = " sent Successfully";
        public static readonly string msgInvalidImageSize = "Image size is not valid";
        public static readonly string msgInvalidImage = "Only png images are valid";
        public static readonly string msgImageFieldRequired = "Image field must required.";
        public static readonly string msgInvalidTime = "Invalid Date or time.";
        public static readonly string msgShiftAlreadyExists = "Shift with provided times already exists. please choose different times";
        public static readonly string msgEnquirySentSuccess = "Thanks for the enquiry.We will contact you soon.";
        public static readonly string msgSuccessfully = " successfully";
        public static readonly string msgToDoListSaved = "Todo Saved";
        public static readonly string msgNoAppointment = "No Appointment exists";
        public static readonly string msgNoavailabledates = "No available dates found";
        public static readonly string msgNoUpcomingAppointments = "No Upcoming Appointments Available";
        public static readonly string msgNoPastAppointments = "No Past Appointments Available";
        public static readonly string msgInvalidScheduleForDeletion  = "This shift cannot be deleted as you have Confirmed Appointments in this shift ";
        public static readonly string msgInvalidScheduleForEdit = "This shift cannot be edited as you have Confirmed Appointments in this shift ";
        public static readonly string msgNoShiftAvailabe = "No time shift available for selected date ";
        public static readonly string msgNotApplicableForBookAppointment = "You cannot request for appointment without completing profile";
        public static readonly string msgCallShouldInitializeSoon = "Your call should be initialize soon. please wait until front user should accept the call.";
        public static readonly string msgTimeSlotNotAvailableForCall = "You Could not able to call the front user until your time slot is available";
        public static readonly string msgInvalidAmount = "Amount must be greater than 0";
        public static readonly string msgIsAlreadyBooked = "Your appointment already exists on same date and time for another doctor. Please try another date and time. ";

    }

    
}
