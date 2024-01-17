/*using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email , string subject , string htmlMessage)
        {
            return SendMailKitEmailAsync(email , subject , htmlMessage);
        }

        private async Task SendMailKitEmailAsync(string email , string subject , string htmlMessage)
        {
            var smtpHost = _config.GetValue<string>("MailKit:SmtpHost");
            var smtpPort = _config.GetValue<int>("MailKit:SmtpPort");
            var smtpUsername = _config.GetValue<string>("MailKit:SmtpUsername");
            var smtpPassword = _config.GetValue<string>("MailKit:SmtpPassword");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bulky Book" , "rohan.bhakte46@gmail.com"));
            message.To.AddRange(message.To);
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = bodyBuilder.ToMessageBody();

            using(var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpHost , smtpPort , SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername , smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
*/