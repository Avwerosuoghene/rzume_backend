
using AutoMapper;

using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Models.Utilities;

namespace RzumeAPI.Repository
{
    public class ProfileRepository(ApplicationDbContext db,
  IMapper mapper, IUserFileRepository dbUserFile, IEducationRepository dbEducation, IExperienceRepository dbExperience) : IProfileRepository
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IEducationRepository _dbEducation = dbEducation;
        private readonly IExperienceRepository _dbExperience = dbExperience;

        private readonly IMapper _mapper = mapper;

        private readonly IUserFileRepository _dbUserFile = dbUserFile;

        public async Task<GenericResponseDTO> OnboardingFirstStage(OnboardUserFirstStageRequestDTO onboardRequestPayload, string userMail)
        {


            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == userMail.ToLower());
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



            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == userMail.ToLower());
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
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.Equals(userMail, StringComparison.CurrentCultureIgnoreCase));
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

