
using AutoMapper;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class ExperienceRepository : Repository<Experience>, IExperienceRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;





        public ExperienceRepository(ApplicationDbContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;



        }







        public async Task<bool> UpdateAsync(Experience experience)
        {



            _db.Experience.Update(experience);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> BulkUpdateAsync(List<ExperienceDTO> experienceListPayload, string userId)
        {


            List<Experience> experienceModelList = new();

            experienceListPayload.ForEach((ExperienceDTO experienceItem) =>
         {
             Experience experienceModel = _mapper.Map<Experience>(experienceItem);
             experienceModel.UserId = userId;
             experienceModelList.Add(experienceModel);
         });



            _db.Experience.AddRange(experienceModelList);
            await _db.SaveChangesAsync();
            return true;
        }

    }
}