
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RzumeAPI.Models;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Options;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private const string templatePath = @"EmailTemplate/{0}.html";

        private readonly SMTPConfigModel _smtConfig;

        private IConfiguration _configuration;




        public EmailRepository(IOptions<SMTPConfigModel> smtConfig, IConfiguration configuration)
        {
            _smtConfig = smtConfig.Value;
            _configuration = configuration;
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
                throw new Exception("An error occured while sending verification mail");

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


        public async Task SendConfrirmationMail(User user, string token, string otpPurpose, bool isSigin, string clientBaseUrl)
        {

            var secret = _configuration["ApiSettings:Secret"];


            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string> { user.Email },
                Placeholders = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("{{userName}}", user.UserName),
                    new KeyValuePair<string, string>("{{link}}" ,$"{clientBaseUrl}auth/email-confirmation?token={token}"),
                    new KeyValuePair<string, string>("{{introText}}", otpPurpose ),
                    new  KeyValuePair<string, string>("{{isSignin}}", isSigin.ToString() ),

                },
                Subject = "Kindly click the link to validate your email.",
            };

            options.Body = UpdatePlaceHolder(GetEmailBody("EmailConfirm", templatePath), options.Placeholders);
        
                await SendEmail(options);
       

        }
    }
}