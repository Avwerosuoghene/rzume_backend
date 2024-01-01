using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using RzumeAPI.Data;
using RzumeAPI.Helpers;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class OtpRepository : Repository<Otp>, IOtpRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;





        public OtpRepository(ApplicationDbContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;

            // _dbOtp = dbOtp;


        }

        





        public async Task<Otp> UpdateAsync(Otp otp)
        {



            _db.Otp.Update(otp);
            await _db.SaveChangesAsync();
            return otp;
        }

    }
}