
using AutoMapper;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class EducationRepository : Repository<Education>, IEducationRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;





        public EducationRepository(ApplicationDbContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;



        }







        public async Task<bool> UpdateAsync(Education education)
        {



            _db.Education.Update(education);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> BulkUpdateAsync(List<EducationDTO> educationListPayload, string userId)
        {


            List<Education> educationModelList = new();

               educationListPayload.ForEach((EducationDTO educationItem) =>
            {
                Education educationModel = _mapper.Map<Education>(educationItem);
                educationModel.UserId = userId;
                educationModelList.Add(educationModel);
            });



            _db.Education.AddRange(educationModelList);
            await _db.SaveChangesAsync();
            return true;
        }



    }
}