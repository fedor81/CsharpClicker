using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace CSharpClicker.Web.UseCases.Email
{
    public class EmailService(string fromName, string fromAddress, string smtpServer, int smtpPort, string smtpLogin, string smtpPassword)
    {
        private readonly string fromName = fromName;
        private readonly string fromAddress = fromAddress;
        private readonly string smtpServer = smtpServer;
        private readonly int smtpPort = smtpPort;
        private readonly string smtpLogin = smtpLogin;
        private readonly string smtpPassword = smtpPassword;

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(fromName, fromAddress));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, true);
                await client.AuthenticateAsync(smtpLogin, smtpPassword);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
