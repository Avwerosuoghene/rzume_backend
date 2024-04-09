
using AutoMapper;
using RzumeAPI.Data;

using RzumeAPI.Models;
using RzumeAPI.Repository.IRepository;


namespace RzumeAPI.Repository
{
    public class UserFileRepository : Repository<UserFile>, IUserFileRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;





        public UserFileRepository(ApplicationDbContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;

        }

        





        public async Task<UserFile> UpdateAsync(UserFile file)
        {



            _db.Userfile.Update(file);
            await _db.SaveChangesAsync();
            return file;
        }

    }
}