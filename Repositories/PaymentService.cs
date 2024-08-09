using AnfasAPI.Data;
using AnfasAPI.Helpers;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class PaymentService : IPaymentService
    {
        private ApplicationDbContext _context;
        private readonly IDoctorService _doctorService;
        private readonly ChargeService _chargeService;
        private readonly CustomerService _customerService;
        private PaymentSettings _paymentSettings { get; }
        public PaymentService(ApplicationDbContext context, ChargeService chargeService, CustomerService customerService, IOptions<PaymentSettings> paymentSettings, IDoctorService doctorService)
        {
            _context = context;
            _chargeService = chargeService;
            _customerService = customerService;
            _paymentSettings = paymentSettings.Value;
            _doctorService = doctorService;
        }

        public PaymentResponseViewModel ProcessPayment(PaymentViewModel model, string userId)
        {
            try
            {
                UserPaymentInfo userPaymentInfo = new UserPaymentInfo();
                PaymentResponseViewModel response = PaymentWithStripe(model);
                if(response.PaymentStatus == "Failed")
                {
                    return response;
                }
                else
                {
                    var _appointmentDetails = _context.Appointments.Where(x => x.Id == model.AppointmentId && x.PatientId == userId).FirstOrDefault();
                    userPaymentInfo.AppointmentId = model.AppointmentId;
                    userPaymentInfo.PatientId = userId;
                    userPaymentInfo.DoctorId = _appointmentDetails.DoctorId;
                    userPaymentInfo.Amount = response.Amount;
                    userPaymentInfo.Currency = response.Currency;
                    userPaymentInfo.TransactionId = response.TransactionId;
                    userPaymentInfo.CustomerId = response.CustomerId;
                    userPaymentInfo.PaymentMode = response.PaymentMode; // Stripe
                    if (response.TransactionType == "credit")
                    {
                        userPaymentInfo.TransactionType = (int)Common.GlobalVariables.TransactionType.Credit;
                    }
                    if (response.PaymentStatus == "succeeded")
                    {
                        userPaymentInfo.PaymentStatus = (int)Common.GlobalVariables.PaymentStatus.Success;
                    }
                    userPaymentInfo.CreatedDate = DateTime.UtcNow;
                    _context.UserPaymentInfo.Add(userPaymentInfo);
                    _context.SaveChanges();
                    return response;
                }
                
                
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PaymentResponseViewModel PaymentWithStripe(PaymentViewModel model)
        {
            PaymentResponseViewModel responseModel = new PaymentResponseViewModel();
            StripeConfiguration.ApiKey = _paymentSettings.StripePrivateKey;
            // StripeConfiguration.ApiKey = "sk_test_51HoioWBMOvSrx6FIlwnSK46xfSRE8ywoODrYa3kPSb6AVWpOlE4YGa5U9Ywo6rE3znkK3mzTekmn9Ik8vimxvG8M0059iTPaYP";
            try
            {
                var customer = _customerService.Create(new CustomerCreateOptions
                {
                    Email = model.Email,
                    Source = model.StripeToken
                });
                var amt = model.Amount * 100; // stripe only accept cents
                var myCharge = new ChargeCreateOptions
                {
                    Amount = amt,
                    Currency = "aud",
                    Description = "Payment With Stripe API Call",
                    Customer = customer.Id
                };
                var stripeCharge = _chargeService.Create(myCharge);
                responseModel.Amount = (decimal)(stripeCharge.Amount / 100);
                responseModel.Currency = stripeCharge.Currency;
                responseModel.TransactionId = stripeCharge.BalanceTransactionId;
                responseModel.CustomerId = stripeCharge.CustomerId;
                responseModel.PaymentMode = stripeCharge.CalculatedStatementDescriptor; // Stripe
                responseModel.PaymentStatus = stripeCharge.Status;
                responseModel.TransactionType = stripeCharge.PaymentMethodDetails.Card.Funding;
                responseModel.FailureCode = stripeCharge.FailureCode;
                responseModel.FailureMessage = stripeCharge.FailureMessage;
                responseModel.PaymentDate = stripeCharge.Created;
                return responseModel;
            }
            catch (StripeException ex)
            {
                responseModel.Amount = (decimal)model.Amount;
                responseModel.Currency = "aud";
                responseModel.PaymentStatus = "Failed";
                responseModel.PaymentDate = DateTime.UtcNow;
                switch (ex.StripeError.Type)
                {
                    case "card_error":
                        responseModel.FailureCode = ex.StripeError.Code;
                        responseModel.FailureMessage = ex.StripeError.Message;
                        break;

                    case "api_connection_error":
                        responseModel.FailureCode = ex.StripeError.Code;
                        responseModel.FailureMessage = ex.StripeError.Message;
                        break;
                    case "api_error":
                        responseModel.FailureCode = ex.StripeError.Code;
                        responseModel.FailureMessage = ex.StripeError.Message;
                        break;
                    case "authentication_error":
                        responseModel.FailureCode = ex.StripeError.Code;
                        responseModel.FailureMessage = ex.StripeError.Message;
                        break;
                    case "invalid_request_error":
                        responseModel.FailureCode = ex.StripeError.Code;
                        responseModel.FailureMessage = ex.StripeError.Message;
                        break;
                    case "rate_limit_error":
                        responseModel.FailureCode = ex.StripeError.Code;
                        responseModel.FailureMessage = ex.StripeError.Message;
                        break;
                    case "validation_error":
                        responseModel.FailureCode = ex.StripeError.Code;
                        responseModel.FailureMessage = ex.StripeError.Message;
                        break;
                    default:
                        // Unknown Error Type
                        break;

                }
                return responseModel;
            }
        }
        public bool ProcessPayout(string userId)
        {
            try
            {
                var _bankDetails =  _doctorService.GetDoctorBankInfo(userId);
                StripeConfiguration.ApiKey = _paymentSettings.StripePrivateKey;
                //StripeConfiguration.ApiKey = "sk_test_51Hqv3vEY974dx7DZnG88AmoFkNbwpb6vCndFgqim7h5LIKWuqdETkiwgrw8nQn2oz2EARZ5mlqwbijlTRbUet1Tx00aGqFVNHe";
                var options = new PayoutCreateOptions
                {
                    Amount = 1100,
                    Currency = "aud",
                    Destination = ""

                };
                var service = new PayoutService();
                service.Create(options);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

    }
}

