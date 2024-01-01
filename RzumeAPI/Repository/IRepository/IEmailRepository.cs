using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RzumeAPI.Models;

namespace RzumeAPI.Repository.IRepository
{
    public interface IEmailRepository
    {
            // Task SendTestEmail(UserEmailOptions userEmailOptions); 

            Task SendConfrirmationMail(User user, string token, string otpPurpose, bool isSigin); 

    }
}