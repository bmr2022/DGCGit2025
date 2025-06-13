using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using eTactWeb.Services.Interface;

namespace eTactWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string emailTo, string subject, string CC1, string CC2, string CC3, string message, byte[] attachment = null, string attachmentName = null)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(emailSettings["FromName"], emailSettings["FromEmail"]));
            mimeMessage.To.Add(MailboxAddress.Parse(emailTo));
            mimeMessage.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = message;

            if (attachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                builder.Attachments.Add(attachmentName, attachment);
            }

            mimeMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(emailSettings["SmtpServer"],
                        int.Parse(emailSettings["SmtpPort"]),
                        MailKit.Security.SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(emailSettings["SmtpUsername"],
                        emailSettings["SmtpPassword"]);

                    await client.SendAsync(mimeMessage);
                }
                catch (Exception ex)
                {
                    // Handle exception
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
