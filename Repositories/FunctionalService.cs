using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.Helpers;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class FunctionalService: IFunctionalService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;  
        private readonly SuperAdminDefaultOptions _superAdminDefaultOptions;

        public FunctionalService(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context,  
           IOptions<SuperAdminDefaultOptions> superAdminDefaultOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;  
            _superAdminDefaultOptions = superAdminDefaultOptions.Value;
        }
         
        public async Task SendEmailBySendGridAsync(string apiKey,
            string fromEmail,
            string fromFullName,
            string subject,
            string message,
            string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromFullName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email, email));
            await client.SendEmailAsync(msg); 
        }

        public async Task SendEmailByGmailAsync(string fromEmail,
            string fromFullName,
            string subject,
            string messageBody,
            string toEmail,
            string toFullName,
            string smtpUser,
            string smtpPassword,
            string smtpHost,
            int smtpPort,
            bool smtpSSL)
        {
            var body = messageBody;
            var message = new MailMessage();
            message.To.Add(new MailAddress(toEmail, toFullName));
            message.From = new MailAddress(fromEmail, fromFullName);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = smtpUser,
                    Password = smtpPassword
                };
                smtp.Credentials = credential;
                smtp.Host = smtpHost;
                smtp.Port = smtpPort;
                smtp.EnableSsl = smtpSSL;
                smtp.UseDefaultCredentials = false;
                await smtp.SendMailAsync(message); 
            } 
        }

        public async Task CreateDefaultSuperAdmin()
        {
            try
            { 
                ApplicationUser superAdmin = new ApplicationUser();
                superAdmin.Email = _superAdminDefaultOptions.Email;
                superAdmin.UserName = _superAdminDefaultOptions.Email;
                superAdmin.NormalizedUserName = _superAdminDefaultOptions.Email;
                superAdmin.NormalizedEmail = _superAdminDefaultOptions.Email; 
                superAdmin.EmailConfirmed = true; 
                superAdmin.FirstName = "Super";
                superAdmin.LastName = "Admin";  
                superAdmin.CreatedDate = DateTime.UtcNow;
                await _userManager.CreateAsync(superAdmin, _superAdminDefaultOptions.Password);

                //loop all the roles
                foreach (GlobalVariables.UserRole item in (GlobalVariables.UserRole[])Enum.GetValues(typeof(GlobalVariables.UserRole)))
                {
                    var roleName = item.ToString();
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
                await _userManager.AddToRoleAsync(superAdmin, GlobalVariables.UserRole.SuperAdmin.ToString());
            }
            catch (Exception)
            { 
                throw;
            }
        }
    }
}
