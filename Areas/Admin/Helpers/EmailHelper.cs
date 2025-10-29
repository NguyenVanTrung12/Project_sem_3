

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Project_sem_3.Areas.Admin.Helpers
{
    public static class EmailHelper
    {
        private static readonly string fromEmail = "organizationwebster@gmail.com";
        private static readonly string appPassword = "jbecmwcuxzmucahd";

        public static async Task SendCandidateAccountEmail(string candidateEmail, string username, string password)
        {
            if (string.IsNullOrEmpty(candidateEmail)) return;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Online Aptitude Test System", fromEmail));
            message.To.Add(MailboxAddress.Parse(candidateEmail));
            message.Subject = "Candidate Account Information";
            message.Body = new TextPart("html")
            {
                Text = $@"
                    Hello,<br/><br/>
                    An account has been created for you in the system.<br/>
                    <b>Username:</b> {username}<br/>
                    <b>Password:</b> {password}<br/><br/>
                    Please log in and change your password after your first access.<br/><br/>
                    Best regards,<br/>
                    Recruitment Department."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromEmail, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
