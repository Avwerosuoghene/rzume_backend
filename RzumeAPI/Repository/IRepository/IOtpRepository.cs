using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RzumeAPI.Models;

namespace RzumeAPI.Repository.IRepository
{
    public interface IOtpRepository: IRepository<Otp>
    {
           Task<Otp> UpdateAsync(Otp entity);

    }
}