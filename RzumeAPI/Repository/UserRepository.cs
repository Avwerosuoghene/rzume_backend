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
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IEmailRepository _emailService;
        private string secretKey;
        private readonly IMapper _mapper;


        private IOtpRepository _dbOtp;

        private readonly MiscellaneousHelper _helperService;






        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IEmailRepository emailService, IOtpRepository dbOtp, MiscellaneousHelper helperService)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
            _dbOtp = dbOtp;
            _helperService = helperService;



        }

        public async Task<UserDTO>? Register(RegistrationDTO registrationDTO)
        {
            User user = new()
            {
                Email = registrationDTO.Email,
                NormalizedEmail = registrationDTO.Email.ToUpper(),
                UserName = registrationDTO.Email,
                TwoFactorEnabled = true,

            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationDTO.Password);

                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers
                .FirstOrDefault(u => u.UserName == registrationDTO.Email);

                    if (userToReturn != null)
                    {
                        await GenerateMail(userToReturn, "Signup", true);
                        return _mapper.Map<UserDTO>(userToReturn);
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve the created user.");
                    }



                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        // Log or handle each error as needed
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return null;
        }

        public async Task<GetActiveUserResponse> GetActiveUser(string token)
        {
            try
            {

                var key = Encoding.ASCII.GetBytes(secretKey);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claimsPrincipal = tokenHandler.ValidateToken(token, validations, out var tokenSecure);
                var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var userMail = userIdClaim?.Value;
                if (userMail == null)
                {
                    return new GetActiveUserResponse()
                    {
                        User = null,
                        Message = "Invalid Request"
                    };
                }

                var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());

                if (user == null)
                {
                    return new GetActiveUserResponse()
                    {
                        User = null,
                        Message = "User not found"
                    };
                }

                return new GetActiveUserResponse()
                {


                    User = _mapper.Map<UserDTO>(user),
                    Message = "Success"

                };



            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine($"Token expired: {ex.Message}");

                return new GetActiveUserResponse()
                {
                    User = null,
                    Message = "Token expired"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new GetActiveUserResponse()
                {
                    User = null,
                    Message = "An error occurred"
                };
            }

        }


        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private async Task GenerateMail(User user, string otpPurpose, bool isSigin)
        {
            var token = MiscellaneousHelper.GenerateOtp();
            DateTime currentDate = DateTime.Now;
            DateTime expirationDate = currentDate.AddMinutes(5);



            OtpDTO otp = new OtpDTO
            {
                UserId = user!.Id.ToString(),
                ExpirationDate = expirationDate,
                OtpValue = token.ToString(),
                IsConfirmed = false

            };

            Otp otpModel = _mapper.Map<Otp>(otp);

            await _dbOtp.CreateAsync(otpModel);



            await _emailService.SendConfrirmationMail(user, token.ToString(), otpPurpose, isSigin);
        }





        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {

            // The async version of this method is FirstOrDefaultAsync
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            bool isValid = false;

            if (user != null)
            {
                isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            }

            if (user == null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    //WeakReference couldn't use the token directly as it is of type SecurityToken
                    Token = "",
                    User = null,
                    Message = "Username or password is incorrect"
                };
            }

            if (!user.EmailConfirmed)
            {
                // await GenerateMail(user!, "Signup", true);
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = _mapper.Map<UserDTO>(user),
                    EmailConfirmed = false,
                    Message = "Kindly Validate User"
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Describes what our token will contain
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email.ToString())
                }),

                //Describes Token Expiration
                Expires = DateTime.UtcNow.AddDays(7),

                //Describes signin creddentials
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // var token = tokenHandler.CreateToken(tokenDescriptor);
            var token = MiscellaneousHelper.GenerateToken(user.Id.ToString(), user.Email.ToString());

            await _userManager.SetAuthenticationTokenAsync(user, "Login", "LoginToken", token);


            //    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);


            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = token,
                User = _mapper.Map<UserDTO>(user),
                EmailConfirmed = true,
                Message = "Login Succesful"

            };


            return loginResponseDTO;

        }


        public async Task<bool> Logout(LogoutRequestDTO logoutRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == logoutRequestDTO.UserName.ToLower());
            if (user == null)
            {
                return false;
            }
            await _userManager.RemoveAuthenticationTokenAsync(user, "Login", "LoginToken");
            return true;

        }

        public async Task<OtpPasswordResetRequestResponseDTO> InitiateOtpResetPassword(OtpPasswordResetRequestDTO passwordResetRequestModel)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == passwordResetRequestModel.Email.ToLower());
            OtpPasswordResetRequestResponseDTO passwordResetResponse = new OtpPasswordResetRequestResponseDTO();
            if (user == null)
            {
                passwordResetResponse.isSuccess = false;
                passwordResetResponse.message = "User not found";
                return passwordResetResponse;
            }

            var otpModel = await _dbOtp.GetAsync(u => u.UserId == user.Id);

            OtpValidationResponseDTO otpConfirmedResponse = await _helperService.ConfirmOtp(user, passwordResetRequestModel.OtpValue.ToString()!);

            if (!otpConfirmedResponse.isValid)
            {
                passwordResetResponse.isSuccess = false;
                passwordResetResponse.message = otpConfirmedResponse.message;
                return passwordResetResponse;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);


            var result = await _userManager.ResetPasswordAsync(user, resetToken, passwordResetRequestModel.Password);

            if (result.Succeeded)
            {
                passwordResetResponse.isSuccess = true;
                passwordResetResponse.message = "Password Reset Succesful";
                return passwordResetResponse;
            }
            else
            {
                passwordResetResponse.isSuccess = false;
                passwordResetResponse.message = "Password Reset Failed";
                return passwordResetResponse;
            }





        }

        public async Task<IdentityResult> ConfirmEmail(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);

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

