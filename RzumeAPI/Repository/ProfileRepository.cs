
using AutoMapper;

using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Models.DO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;
using RzumeAPI.Services;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace RzumeAPI.Repository
{
    public class ProfileRepository(ApplicationDbContext db,
  IMapper mapper, IUserFileRepository dbUserFile, IEducationRepository dbEducation, IExperienceRepository dbExperience, TokenService tokenService, IEmailRepository emailService, IUserRepository userRepo, UserManager<User> userManager, UserService userService) : IProfileRepository
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IEducationRepository _dbEducation = dbEducation;
        private readonly IExperienceRepository _dbExperience = dbExperience;

        private readonly TokenService _tokenService = tokenService;

        private readonly IEmailRepository _emailService = emailService;

        private readonly IUserRepository _userRepo = userRepo;

        private readonly UserManager<User> _userManager = userManager;

        private readonly UserService _userService = userService;






        private readonly IMapper _mapper = mapper;

        private readonly IUserFileRepository _dbUserFile = dbUserFile;

        public async Task<GenericResponse> OnboardingFirstStage(OnboardUserFirstStageRequest onboardRequestPayload, string token)
        {



            GetUserFromTokenResponse response = await _userService.GetUserFromToken(token);

            User? user = response.User;

            if (user == null)
            {
                return GenerateErrorResponse(UserStatMsg.NotFound);
            }
            user.UserName = onboardRequestPayload.UserName;
            user.OnBoardingStage = 1;
            await UpdateAsync(user);

            return GenerateSuccessResponse(SuccessMsg.Updated);
        }

        public async Task<GenericResponse> OnboardingSecondStage(OnboardUserSecondStageRequest onboardRequestPayload, string token)
        {



            GetUserFromTokenResponse response = await _userService.GetUserFromToken(token);

            User? user = response.User; if (user == null)
            {
                return GenerateErrorResponse(UserStatMsg.NotFound);
            }




            UserFileDTO file = new()
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


        public async Task<GenericResponse> OnboardingThirdStage(OnboardUserThirdStageRequest onboardRequestPayload, string token)
        {
            GetUserFromTokenResponse response = await _userService.GetUserFromToken(token);

            User? user = response.User; if (user == null)
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


        public async Task<GenericResponse> OnboardingFourthStage(OnboardUserFourthStageRequest onboardRequestPayload, string token)
        {
    GetUserFromTokenResponse response = await _userService.GetUserFromToken(token);

            User? user = response.User;            if (user == null)
            {
                return GenerateErrorResponse(UserStatMsg.NotFound);
            }

            await _dbExperience.BulkUpdateAsync(onboardRequestPayload.ExperienceList, user.Id);

            return GenerateSuccessResponse("experience updated succesfully");
        }


        public async Task<GenericResponse> RequestPasswordReset(RequestPasswordReset requestPasswordRequest, string clientSideBaseUrl)
        {




            var user = _db.ApplicationUsers.Where(u => u.Email != null).FirstOrDefault(u => u.Email.ToLower() == requestPasswordRequest.Email.ToLower());
            if (user == null)
            {
                {
                    return GenerateErrorResponse(UserStatMsg.NotFound);
                }

            }

            if (!user.EmailConfirmed)
            {
                {
                    return GenerateErrorResponse(UserStatMsg.EmailNotConfirmedMsg);
                }
            }



            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);






            await GenerateMail(user, TokenTypes.ResetPass, false, clientSideBaseUrl, resetToken);
            return GenerateSuccessResponse("Password reset succesfully initiated. kindly check your email for instructions");

        }



        public async Task<GenericResponse> ResetPassword(ResetPassword resetPasswordPayload)
        {


            User? user = await _userRepo.GetUserByEmailAsync(resetPasswordPayload.Email);

            if (user == null)
            {
                return new GenericResponse()
                {
                    IsSuccess = false,
                    Message = UserStatMsg.NotFound
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordPayload.ResetToken, resetPasswordPayload.Password);


            if (result.Succeeded)
            {
                return GenerateSuccessResponse(PasswordStatMsgs.SuccesfulReset);
            }

            return GenerateErrorResponse(PasswordStatMsgs.FailedReset);






        }




        public async Task<User> UpdateAsync(User user)
        {



            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }



        private static GenericResponse GenerateErrorResponse(string message)
        {
            return new GenericResponse
            {
                IsSuccess = false,
                Message = message
            };
        }

        private static GenericResponse GenerateSuccessResponse(string message)
        {
            return new GenericResponse
            {
                IsSuccess = true,
                Message = message
            };
        }


        private async Task GenerateMail(User user, string mailPurpose, bool isSigin, string clientBaseUrl, string token)
        {

            string encodedToken = WebUtility.UrlEncode(token);
            string encodedEmail = WebUtility.UrlEncode(user.Email!);

            string linkPath = $"{clientBaseUrl}auth/reset-password/{encodedToken}/{encodedEmail}";
            string templatePath = @"EmailTemplate/{0}.html";
            string templateName = "ResetPassConfirm";
            string mailSubject = "Kindly click the link to reset your password.";
            SendConfirmEmailProps confirmMailProps = new()
            {
                User = user,
                Token = token,
                MailPurpose = mailPurpose,
                IsSigin = isSigin,
                LinkPath = linkPath,
                TemplatePath = templatePath,
                TemplateName = templateName,
                Subject = mailSubject,
            };


            await _emailService.SendConfrirmationMail(confirmMailProps);
        }
    }




}

