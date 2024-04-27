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

            NetworkCredential networkCredential = new NetworkCredential(_smtConfig.UserName, _smtConfig.Password);

            SmtpClient smtpClient = new()
            {
                EnableSsl = _smtConfig.EnableSSL,
                Host = _smtConfig.Host,
                Port = _smtConfig.Port,
                Credentials = networkCredential
            };

            mail.BodyEncoding = Encoding.Default;
            try
            {
                await smtpClient.SendMailAsync(mail);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private string GetEmailBody(string templateName, string templatePath)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            ;
            return body;
        }

        private static string UpdatePlaceHolder(string text, List<KeyValuePair<string, string>> keyValuePairs)
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

        // public async Task SendTestEmail(UserEmailOptions userEmailOptions, string templatePath)
        // {
        //     userEmailOptions.Subject = UpdatePlaceHolder("This is a test email from {{userName}} ", userEmailOptions.Placeholders);
        //     userEmailOptions.Body = UpdatePlaceHolder(GetEmailBody("TestEmail", templatePath), userEmailOptions.Placeholders);

        //     await SendEmail(userEmailOptions);
        // }

        public async Task SendConfrirmationMail(User user, string token, string otpPurpose, bool isSigin)
        {


            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string> { user.Email },
                Placeholders = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("{{userName}}", user.UserName),
                    new KeyValuePair<string, string>("{{link}}", token ),
                    new KeyValuePair<string, string>("{{introText}}", otpPurpose ),
                    new  KeyValuePair<string, string>("{{isSignin}}", isSigin.ToString() ),

                },
                Subject = "Kindly confrim your email id.",
            };

            options.Body = UpdatePlaceHolder(GetEmailBody("EmailConfirm", templatePath), options.Placeholders);

            await SendEmail(options);
        }
    }
}