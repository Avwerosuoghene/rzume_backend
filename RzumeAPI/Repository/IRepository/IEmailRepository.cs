
using RzumeAPI.Models.Requests;

namespace RzumeAPI.Repository.IRepository
{
    public interface IEmailRepository
    {
        Task SendConfrirmationMail(SendConfirmEmailProps confirmationProps);
    }
}