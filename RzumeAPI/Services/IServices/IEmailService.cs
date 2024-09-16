
using RzumeAPI.Models;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Utilities;

namespace RzumeAPI.Services.IServices;
public interface IEmailService
{

    Task GenerateMail(User user, string mailPurpose, bool isSigin, string clientBaseUrl, string token);

    Task SendEmail(UserEmailOptions userEmailOptions);

    Task SendConfrirmationMail(SendConfirmEmailProps confirmationProps);
}