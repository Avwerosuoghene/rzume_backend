using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RzumeAPI.Models;
using RzumeAPI.Options;

namespace RzumeAPI.Repository.IRepository
{
    public interface IEmailRepository
    {
            // Task SendTestEmail(UserEmailOptions userEmailOptions); 

            Task SendConfrirmationMail(User user, string token, string otpPurpose, bool isSigin, string clientBaseUrl); 

    }
}