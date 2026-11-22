
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace UrbanHazardMap.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(
            IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(
            string toEmail,
            string toName,
            string subject,
            string htmlBody,
            EmailAttachment? attachment = null)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

            message.To.Add(new MailboxAddress(
                toName,
                toEmail));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            if (attachment != null)
            {
                var contentType = ContentType.Parse(
                    attachment.ContentType);

                bodyBuilder.Attachments.Add(
                attachment.FileName,
                attachment.Content,
                contentType);
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            int timeoutSeconds =
                _settings.TimeoutSeconds > 0
                    ? _settings.TimeoutSeconds
                    : 10;

            client.Timeout = checked(timeoutSeconds * 1000);

            var security = _settings.UseStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            // Enforce an overall deadline, not just
            // a timeout for individual network operations.
            using var cancellation = new CancellationTokenSource(
                TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                await client.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    security,
                    cancellation.Token);

                if (!string.IsNullOrWhiteSpace(
                    _settings.Username))
                {
                    await client.AuthenticateAsync(
                        _settings.Username,
                        _settings.Password,
                        cancellation.Token);
                }

                await client.SendAsync(
                    message,
                    cancellation.Token);

                await client.DisconnectAsync(
                    true,
                    cancellation.Token);
            }
            catch (OperationCanceledException ex)
                when (cancellation.IsCancellationRequested)
            {
                throw new TimeoutException(
                    $"SMTP operation exceeded " +
                    $"{timeoutSeconds} seconds.",
                    ex);
            }
        }
    }
}
