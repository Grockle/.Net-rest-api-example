using System.Threading.Tasks;
using BackendService.Exceptions;
using BackendService.Models;
using BackendService.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace BackendService.Helpers.Implementations
{
    public class EmailService : IEmailService
    {
        public MailSettings MailSettings { get; }
        public ILogger<EmailService> Logger { get; }

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            MailSettings = mailSettings.Value;
            Logger = logger;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                // create message
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(request.From ?? MailSettings.EmailFrom),
                    To = { MailboxAddress.Parse(request.To) },
                    Subject = request.Subject,
                    Body = new TextPart(TextFormat.Html) { Text = request.Body }
                };

                // send email
                using var smtp = new SmtpClient
                {
                    ServerCertificateValidationCallback = (s, c, h, e) => true
                };
                await smtp.ConnectAsync(MailSettings.SmtpHost, MailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(MailSettings.SmtpUser, MailSettings.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                throw new ApiException(ex.Message);
            }
        }
    }
}
