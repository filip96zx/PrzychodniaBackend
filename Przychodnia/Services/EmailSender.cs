using MimeKit;
using Przychodnia.Interfaces;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Przychodnia.Transfer.Email;

namespace Przychodnia.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly EmailSetting _emailSetting;
        private readonly IHostingEnvironment _environment;

        public EmailSender(IOptions<EmailSetting> emailOptions,
            IHostingEnvironment environment)
        {
            _emailSetting = emailOptions.Value;
            _environment = environment;
        }

        public async Task SendEmailAsync(string email, string subject, MimeMessage message)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSetting.SenderName, _emailSetting.Sender));

                mimeMessage.To.Add(new MailboxAddress(email));

                mimeMessage.Subject = subject;

                mimeMessage.Body = message.Body;
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    if (_environment.IsDevelopment())
                    {
                        await client.ConnectAsync(_emailSetting.MailServer, _emailSetting.MailPort, true);
                    }
                    else
                    {
                        await client.ConnectAsync(_emailSetting.MailServer);
                    }

                    await client.AuthenticateAsync(_emailSetting.Sender, _emailSetting.Password);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }

}
