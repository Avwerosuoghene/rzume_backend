
using AutoMapper;

using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Models.Utilities;

namespace RzumeAPI.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IEducationRepository _dbEducation;
        private readonly IExperienceRepository _dbExperience;

        private readonly IMapper _mapper;

        private readonly IUserFileRepository _dbUserFile;
        
        public ProfileRepository(ApplicationDbContext db,
  IMapper mapper, IUserFileRepository dbUserFile, IEducationRepository dbEducation, IExperienceRepository dbExperience)
        {
            _db = db;

            _dbUserFile = dbUserFile;

            _mapper = mapper;

            _dbEducation = dbEducation;

            _dbExperience = dbExperience;




        }




        public async Task<GenericResponseDTO> OnboardingFirstStage(OnboardUserFirstStageRequestDTO onboardRequestPayload, string userMail)
        {


            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                return GenerateErrorResponse(UserStatMsg.NotFound);
            }
            user.UserName = onboardRequestPayload.UserName;
            user.OnBoardingStage = 1;
            await UpdateAsync(user);

            return GenerateSuccessResponse(SuccessMsg.Updated);
        }

        public async Task<GenericResponseDTO> OnboardingSecondStage(OnboardUserSecondStageRequestDTO onboardRequestPayload, string userMail)
        {



            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                return GenerateErrorResponse(UserStatMsg.NotFound);
            }




            UserFileDTO file = new UserFileDTO
            {
                FileName = onboardRequestPayload.FileName,
                FileCategory = onboardRequestPayload.FileCat.ToString(),
                FileBytes = onboardRequestPayload.FileBytes,
                UserId = user!.Id.ToString(),

            };

            UserFile userFileModel = _mapper.Map<UserFile>(file);

            await _dbUserFile.CreateAsync(userFileModel);
            return GenerateSuccessResponse(SuccessMsg.Uploaded);

        }


        public async Task<GenericResponseDTO> OnboardingThirdStage(OnboardUserThirdStageRequestDTO onboardRequestPayload, string userMail)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                return GenerateErrorResponse(UserStatMsg.NotFound);
            }

            List<Education> userEducationItems = _db.Education.Where(education => education.UserId == user.Id).ToList();

            if (userEducationItems.Count >= 3)

            {
                return GenerateErrorResponse(EducationStatMsgs.MaximumExceeded);
            }



            List<EducationDTO> receivedEducationList = onboardRequestPayload.EducationList;



            if (receivedEducationList.Count + userEducationItems.Count > 3)
            {
                return GenerateErrorResponse(EducationStatMsgs.MaximumRecords);
            }

            await _dbEducation.BulkUpdateAsync(onboardRequestPayload.EducationList, user.Id);

            return GenerateSuccessResponse(SuccessMsg.Updated);
        }


   public async Task<GenericResponseDTO> OnboardingFourthStage(OnboardUserFourthStageRequestDTO onboardRequestPayload, string userMail)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                return GenerateErrorResponse(UserStatMsg.NotFound);
            }

            await _dbExperience.BulkUpdateAsync(onboardRequestPayload.ExperienceList, user.Id);

            return GenerateSuccessResponse("experience updated succesfully");
        }


        public async Task<User> UpdateAsync(User user)
        {



            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }



        private GenericResponseDTO GenerateErrorResponse(string message)
        {
            return new GenericResponseDTO
            {
                IsSuccess = false,
                Message = message
            };
        }

        private GenericResponseDTO GenerateSuccessResponse(string message)
        {
            return new GenericResponseDTO
            {
                IsSuccess = true,
                Message = message
            };
        }
    }




}

