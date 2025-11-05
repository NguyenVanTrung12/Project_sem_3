using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project_sem_3.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            string fromEmail = emailSettings["From"];
            string password = emailSettings["Password"];
            string host = emailSettings["Host"];
            int port = int.Parse(emailSettings["Port"]);

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(fromEmail, password);

                var message = new MailMessage(fromEmail, toEmail, subject, body);
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);
            }
        }
    }
}
