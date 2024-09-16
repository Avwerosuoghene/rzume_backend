
using AutoMapper;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class OtpRepository : Repository<Otp>, IOtpRepository
    {

        private readonly ApplicationDbContext _db;




        public OtpRepository(ApplicationDbContext db, IMapper mapper) : base(db)
        {
            _db = db;
        }

        

        public async Task<Otp> UpdateAsync(Otp otp)
        {



            _db.Otp.Update(otp);
            await _db.SaveChangesAsync();
            return otp;
        }

    }
}