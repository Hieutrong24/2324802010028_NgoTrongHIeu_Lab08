using ASC.Web.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ASC.Web.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender, Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public AuthMessageSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_emailSettings.SenderPassword) ||
                _emailSettings.SenderEmail == "your-email@gmail.com" ||
                _emailSettings.SenderPassword == "your-gmail-app-password")
            {
                return;
            }

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(
                _emailSettings.SenderName,
                _emailSettings.SenderEmail));

            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("html")
            {
                Text = message
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _emailSettings.SenderEmail,
                _emailSettings.SenderPassword);

            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }

        public Task SendSmsAsync(string number, string message)
        {
            return Task.CompletedTask;
        }
    }
}