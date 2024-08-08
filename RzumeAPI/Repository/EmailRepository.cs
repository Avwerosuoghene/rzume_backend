
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using RzumeAPI.Models;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class EmailRepository(IOptions<SMTPConfigModel> smtConfig, IConfiguration configuration) : IEmailRepository
    {
        private const string templatePath = @"EmailTemplate/{0}.html";

        private readonly SMTPConfigModel _smtConfig = smtConfig.Value;

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

        private static string GetEmailBody(string templateName, string templatePath)
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


        public async Task SendConfrirmationMail(SendConfirmEmailProps confirmationProps)
        {

            


            UserEmailOptions options = new()
            {
                ToEmails = [confirmationProps.User!.Email],
                Placeholders = [
                    new("{{userName}}", confirmationProps.User.UserName!),
                    new KeyValuePair<string, string>("{{link}}" ,confirmationProps.LinkPath),
                    new KeyValuePair<string, string>("{{introText}}",confirmationProps.MailPurpose ),
                    new  KeyValuePair<string, string>("{{isSignin}}", confirmationProps.IsSigin.ToString() ),

                ],
                Subject = confirmationProps.Subject,
            };

            options.Body = UpdatePlaceHolder(GetEmailBody(confirmationProps.TemplateName, confirmationProps.TemplatePath), options.Placeholders);

            await SendEmail(options);


        }

    
    }
}