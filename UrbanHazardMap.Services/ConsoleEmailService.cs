
using Microsoft.Extensions.Logging;

namespace UrbanHazardMap.Services
{
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> _logger;

        public ConsoleEmailService(
            ILogger<ConsoleEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(
            string toEmail,
            string toName,
            string subject,
            string htmlBody,
            EmailAttachment? attachment = null)
        {
            _logger.LogInformation(
                "Email recipient: {ToName} <{ToEmail}>",
                toName,
                toEmail);

            _logger.LogInformation(
                "Email subject: {Subject}",
                subject);

            _logger.LogInformation(
                "Email body: {HtmlBody}",
                htmlBody);

            if (attachment != null)
            {
                _logger.LogInformation(
                    "Email attachment: {FileName}",
                    attachment.FileName);
            }

            return Task.CompletedTask;
        }
    }
}
