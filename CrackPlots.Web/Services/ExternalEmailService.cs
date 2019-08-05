using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ForeSight.Web.Services
{
    public class ExternalEmailService
    {
        public async static Task<bool> ConfirmRegistration(string linkUrl, string email)
        {
            string path = HttpContext.Current.Server.MapPath("~/HTML_Templates/RegistrationEmail.html");
            string emailBody = System.IO.File.ReadAllText(path);
            string finalEmail = emailBody.Replace("{{linkUrl}}", linkUrl);

            string name = "CrackPlots";
            string toName = "";
            string toAddress = email;
            string fromAddress = System.Web.Configuration.WebConfigurationManager.AppSettings["SiteAdminEmailAddress"];
            string subject = "Thank you for registering with CrackPlots!";
            string messageText = "Please click the following link to confirm your email address: " + linkUrl;
            string messageHtml = finalEmail;
            return await Send(fromAddress, name, toName, toAddress, subject, messageText, messageHtml);
        }

        public async static Task<bool> ResetPassword(string linkUrl, string email)
        {
            string path = HttpContext.Current.Server.MapPath("~/HTML_Templates/PasswordResetEmail.html");
            string emailBody = System.IO.File.ReadAllText(path);
            string finalEmail = emailBody.Replace("{{linkUrl}}", linkUrl);

            string name = "CrackPlots";
            string toName = "";
            string toAddress = email;
            string fromAddress = System.Web.Configuration.WebConfigurationManager.AppSettings["SiteAdminEmailAddress"];
            string subject = "Reset Crackplots Password";
            string messageText = "Please click the following link to reset your password: " + linkUrl;
            string messageHtml = finalEmail;
            return await Send(fromAddress, name, toName, toAddress, subject, messageText, messageHtml);
        }

        public async static Task<bool> Send(string Email, string Name, string toName, string toAddress, string subject, string messageText, string messageHtml)
        {
            var apiKey = ConfigurationManager.AppSettings.Get("SendGrid");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Email, Name),
                Subject = subject,
                PlainTextContent = messageText,
                HtmlContent = messageHtml
            };
            msg.AddTo(new EmailAddress(toAddress, toName));
            SendGrid.Response response = await client.SendEmailAsync(msg);
            bool success = response.StatusCode == System.Net.HttpStatusCode.Accepted;
            return success;
        }
    }
}