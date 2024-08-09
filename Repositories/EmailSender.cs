using AnfasAPI.Helpers;
using AnfasAPI.IServices;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class EmailSender : IEmailSender
    {
        private SendGridOptions _sendGridOptions { get; }
        private IFunctionalService _functionalService { get; }
        private SmtpOptions _smtpOptions { get; }

        public EmailSender(IOptions<SendGridOptions> sendGridOptions,
            IFunctionalService functionalService,
            IOptions<SmtpOptions> smtpOptions)
        {
            _sendGridOptions = sendGridOptions.Value;
            _functionalService = functionalService;
            _smtpOptions = smtpOptions.Value;
        }


        public Task SendEmailAsync(string email, string subject, string message)
        {
            //sendgrid is become default
            if (_sendGridOptions.IsDefault)
            {
                _functionalService.SendEmailBySendGridAsync(_sendGridOptions.SendGridKey,
                                                    _sendGridOptions.FromEmail,
                                                    _sendGridOptions.FromFullName,
                                                    subject,
                                                    message,
                                                    email)
                                                    .Wait();
            }

            //smtp is become default
            if (_smtpOptions.IsDefault)
            {
                _functionalService.SendEmailByGmailAsync(_smtpOptions.fromEmail,
                                            _smtpOptions.fromFullName,
                                            subject,
                                            message,
                                            email,
                                            email,
                                            _smtpOptions.smtpUserName,
                                            _smtpOptions.smtpPassword,
                                            _smtpOptions.smtpHost,
                                            _smtpOptions.smtpPort,
                                            _smtpOptions.smtpSSL)
                                            .Wait();
            } 
            return Task.CompletedTask;
        }
    }
}
