using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RzumeAPI.Data;
using RzumeAPI.Helpers;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private ApplicationDbContext _db;

        private readonly IMapper _mapper;

        private IUserFileRepository _dbUserFile;






        public ProfileRepository(ApplicationDbContext db,
          IMapper mapper, IUserFileRepository dbUserFile)
        {
            _db = db;

            _dbUserFile = dbUserFile;

            _mapper = mapper;



        }


        public async Task<GenericResponseDTO> OnboardingFirstStage(OnboardUserFirstStageRequestDTO onboardRequestPayload, string userMail)
        {
            GenericResponseDTO genericResponse = new GenericResponseDTO
            {
                isSuccess = false,
                message = ""
            };

            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                genericResponse.message = "User does not exist";
                return genericResponse;
            }
            user.Name = $"{onboardRequestPayload.FirstName} {onboardRequestPayload.LastName}";
            user.OnBoardingStage = 1;
            await UpdateAsync(user);
            genericResponse.message = "updated succesfully";
            genericResponse.isSuccess = true;

            return genericResponse;
        }

        public async Task<GenericResponseDTO> OnboardingSecondStage(OnboardUserSecondStageRequestDTO onboardRequestPayload, string userMail)
        {
            GenericResponseDTO genericResponse = new GenericResponseDTO
            {
                isSuccess = false,
                message = ""
            };

            Console.WriteLine(onboardRequestPayload.FileCat.ToString());

            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                genericResponse.message = "User does not exist";
                return genericResponse;
            }

            Console.WriteLine(_mapper != null);



            UserFileDTO file = new UserFileDTO
            {
                FileName = onboardRequestPayload.FileName,
                FileCategory = onboardRequestPayload.FileCat.ToString(),
                FileBytes = onboardRequestPayload.FileBytes,
                UserId = user!.Id.ToString(),

            };

            UserFile userFileModel = _mapper.Map<UserFile>(file);

            await _dbUserFile.CreateAsync(userFileModel);
            genericResponse.message = "created succesfully";
            genericResponse.isSuccess = true;

            return genericResponse;
        }


        public async Task<User> UpdateAsync(User user)
        {



            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }




}

