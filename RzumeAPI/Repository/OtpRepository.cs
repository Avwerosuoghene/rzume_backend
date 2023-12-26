using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class OtpRepository : Repository<Otp>,  IOtpRepository
    {

       private readonly ApplicationDbContext _db;

        public OtpRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public async Task<Otp> UpdateAsync(Otp entity)
        {
             _db.Otp.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

    }
}