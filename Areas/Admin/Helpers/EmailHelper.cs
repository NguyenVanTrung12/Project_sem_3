

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
            message.Subject = "Thông tin tài khoản ứng viên";
            message.Body = new TextPart("html")
            {
                Text = $@"
                    Xin chào,<br/><br/>
                    Hệ thống đã tạo tài khoản cho bạn.<br/>
                    <b>Tên đăng nhập:</b> {username}<br/>
                    <b>Mật khẩu:</b> {password}<br/><br/>
                    Vui lòng đăng nhập và đổi mật khẩu sau khi truy cập lần đầu.<br/><br/>
                    Trân trọng,<br/>
                    Bộ phận tuyển dụng."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromEmail, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
