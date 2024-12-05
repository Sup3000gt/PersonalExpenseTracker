using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using UserService.Models;

namespace UserService.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridSettings _sendGridSettings;

        public EmailService(IOptions<SendGridSettings> sendGridSettings)
        {
            _sendGridSettings = sendGridSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            var client = new SendGridClient(_sendGridSettings.ApiKey);
            var from = new EmailAddress("xingalan1992@gmail.com", "PersonalExpenseTracker");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }
    }
}
