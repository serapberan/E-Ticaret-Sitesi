using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SB_Kalem.Email
{
    public class EmailSender : IEmailSender
    {
        public EmailOptions Options { get; set; }
        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(Options.SenGridKey);
            var mesaj = new SendGridMessage()
            {
                From =new EmailAddress("serapberan@hotmail.com","SB_Kalem E-Ticaret Sayfası"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };
            mesaj.AddTo(new EmailAddress(email));
            try
            {
                return client.SendEmailAsync(mesaj);

            }
            catch (Exception)
            {

               // throw;
            }
            return null;

        }
     
    }
}
