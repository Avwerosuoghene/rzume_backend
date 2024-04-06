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
        private readonly UserManager<User> _userManager;
        private readonly IEmailRepository _emailService;
        private string secretKey;
        // private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        // private IOtpRepository _dbOtp;

        private readonly MiscellaneousHelper _helperService;






        public ProfileRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IEmailRepository emailService, IOtpRepository dbOtp, MiscellaneousHelper helperService)
        {
            _db = db;
            // _mapper = mapper;
            // _userManager = userManager;
            // _configuration = configuration;
            // _dbOtp = dbOtp;
            _helperService = helperService;



        }

     
        public async Task<GenericResponseDTO> OnboardingFirstStage(OnboardUserFirstStageRequestDTO onboardRequestPayload, string userMail)
        {
             GenericResponseDTO genericResponse = new GenericResponseDTO{
                isSuccess = false,
                message = ""
             } ;
            
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                genericResponse.message = "User does not exist";
                return genericResponse;
            }
            user.Name = $"{onboardRequestPayload.FirstName} {onboardRequestPayload.LastName}";
            user.OnBoardingStage = 1;
             genericResponse.message = "updated succesfully";
             genericResponse.isSuccess = true;
            await UpdateAsync(user);
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

