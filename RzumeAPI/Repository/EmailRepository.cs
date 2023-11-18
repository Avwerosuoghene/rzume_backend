using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RzumeAPI.Models;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private const string templatePath = @"EmailTemplate/{0}.html";

        private readonly SMTPConfigModel _smtConfig;


        public async Task SendTestEmail(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolder("This is a test email from {{userName}} ", userEmailOptions.Placeholders);
            userEmailOptions.Body = UpdatePlaceHolder(GetEmailBody("TestEmail"), userEmailOptions.Placeholders);

            await SendEmail(userEmailOptions);
        }

        public EmailRepository(IOptions<SMTPConfigModel> smtConfig)
        {
            _smtConfig = smtConfig.Value;
        }

        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            MailMessage mail = new MailMessage
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(_smtConfig.SenderAddress, _smtConfig.SenderDisplayName),
                IsBodyHtml = _smtConfig.IsBodyHTML
            };

            foreach (var toEmail in userEmailOptions.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            NetworkCredential networkCredential = new NetworkCredential(_smtConfig.UserName,_smtConfig.Password);

            SmtpClient smtpClient = new()
            {
                EnableSsl = _smtConfig.EnableSSL,
                Host = _smtConfig.Host,
                Port = _smtConfig.Port,
                // UseDefaultCredentials = _smtConfig.UserDefaulCredentials,
                Credentials = networkCredential
            };

            mail.BodyEncoding = Encoding.Default;

            await smtpClient.SendMailAsync(mail);
        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            ;
            return body;
        }

        private string UpdatePlaceHolder(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var placeholder in keyValuePairs)
                {
                    if (text.Contains(placeholder.Key))
                    {
                        text = text.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }
            return text;
        }
    }
}