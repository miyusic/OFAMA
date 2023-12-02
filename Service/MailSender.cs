using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using OFAMA.Models;

namespace OFAMA.Service
{

    public class MailSender : IEmailSender
    {
        SendMailParams _sendMailParams;
        private readonly ILogger _logger;



        public MailSender(IOptions<SendMailParams> optionAccessor, ILogger<MailSender> logger)
        {
            _sendMailParams = optionAccessor.Value;
            _logger = logger;
        }

        /*public SendMailParams options { get; }*/

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

       
        public async Task Execute(string subject,string message,string email)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_sendMailParams.User, _sendMailParams.SendAddress));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_sendMailParams.MailServer, _sendMailParams.Port, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(_sendMailParams.User,_sendMailParams.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

            

            



        }
    }

}
