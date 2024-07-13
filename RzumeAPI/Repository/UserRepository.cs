
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Services;

namespace RzumeAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IEmailRepository _emailService;
        private readonly IMapper _mapper;


        private IOtpRepository _dbOtp;

        private readonly OtpService _otpService;

        private TokenService _tokenService;






        public UserRepository(ApplicationDbContext db, OtpService otpService,
            UserManager<User> userManager, IMapper mapper, IEmailRepository emailService, IOtpRepository dbOtp, TokenService tokenService)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
            _dbOtp = dbOtp;
            _otpService = otpService;
            _tokenService = tokenService;



        }

        public async Task<string> SendTokenEmailValidation(User user)
        {


            var activeToken = await _userManager.GetAuthenticationTokenAsync(user, "Signup", "SignupToken");
            TokenServiceResponse tokenServiceResponse = _tokenService.ValidateToken(activeToken);
            if (tokenServiceResponse.Message != "Token Expired")
            {
                return "Signup token still active, kindly check your mail";
            }

            await GenerateToken(user, DateTime.UtcNow.AddMinutes(5), TokenNames.SignUp);

            return "Kindly check your mail for your activation token";

        }





        public async Task<RegisterUserResponse> Register(RegistrationDTO registrationDTO, string clientSideBaseUrl)
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
                        // comment back in once google smtp starts working
                        // await GenerateMail(userToReturn, "Signup", true, clientSideBaseUrl);

                        // GenerateMail(userToReturn, "Signup", true, clientSideBaseUrl);

                        string token = await GenerateToken(user, DateTime.UtcNow.AddMinutes(5), TokenNames.SignUp);

                        string validationUrl = $"http://localhost:4200/auth/email-confirmation?token={token}";
                        Console.WriteLine($"validation url is: {validationUrl}");
                        UserDTO returnedUser = _mapper.Map<UserDTO>(userToReturn);
                        return new RegisterUserResponse()
                        {
                            User = returnedUser,
                        };
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

                    throw new Exception("Registration failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                //   throw new Exception("An error occurred during registration.");

                return new RegisterUserResponse()
                {
                    User = null,
                    Message = ex.Message
                };
            }

        }

        public async Task<GetActiveUserResponse> GetActiveUser(string token)
        {
            try
            {


                TokenServiceResponse tokenServiceResponse = _tokenService.ValidateToken(token);
                string? userMail = tokenServiceResponse.UserMail;
                if (userMail == null)
                {
                    return new GetActiveUserResponse()
                    {
                        User = null,
                        Message = tokenServiceResponse.Message
                    };
                }

                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == userMail.ToLower());

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

        // comment back in once google smtp starts working
        // private async Task GenerateMail(User user, string otpPurpose, bool isSigin, string clientBaseUrl)
        // {
        //     DateTime expirationDate = DateTime.UtcNow.AddMinutes(5);
        //     var token = _tokenService.GenerateToken(user.Id, user.Email, expirationDate);
        //     Console.WriteLine($"token value is: {token}");

        //     await _emailService.SendConfrirmationMail(user, token.ToString(), otpPurpose, isSigin, clientBaseUrl);
        // }








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
                    Token = "",
                    User = null,
                    Message = "Username or password is incorrect"
                };
            }

            if (!user.EmailConfirmed)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = _mapper.Map<UserDTO>(user),
                    EmailConfirmed = false,
                    Message = "Kindly Validate User"
                };
            }

            string token = await GenerateToken(user, DateTime.UtcNow.AddHours(1), TokenNames.Login);


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
                passwordResetResponse.IsSuccess = false;
                passwordResetResponse.Message = "User not found";
                return passwordResetResponse;
            }

            var otpModel = await _dbOtp.GetAsync(u => u.UserId == user.Id);

            OtpValidationResponseDTO otpConfirmedResponse = await _otpService.ConfirmOtp(user, passwordResetRequestModel.OtpValue.ToString()!);

            if (!otpConfirmedResponse.IsValid)
            {
                passwordResetResponse.IsSuccess = false;
                passwordResetResponse.Message = otpConfirmedResponse.Message;
                return passwordResetResponse;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);


            var result = await _userManager.ResetPasswordAsync(user, resetToken, passwordResetRequestModel.Password);

            if (result.Succeeded)
            {
                passwordResetResponse.IsSuccess = true;
                passwordResetResponse.Message = "Password Reset Succesful";
                return passwordResetResponse;
            }
            else
            {
                passwordResetResponse.IsSuccess = false;
                passwordResetResponse.Message = "Password Reset Failed";
                return passwordResetResponse;
            }





        }

        public async Task<IdentityResult> ConfirmEmail(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);

        }

        private async Task<string> GenerateToken(User user, DateTime expirationDate, string tokenName)
        {
            var token = _tokenService.GenerateToken(user.Id, user.Email, expirationDate);
            await _userManager.SetAuthenticationTokenAsync(user, tokenName, $"{tokenName}Token", token);
            return token;

        }

        public async Task<GenericResponseDTO> OnboardingFirstStage(OnboardUserFirstStageRequestDTO onboardRequestPayload, string userMail)
        {
            GenericResponseDTO genericResponse = new GenericResponseDTO
            {
                IsSuccess = false,
                Message = ""
            };

            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == userMail.ToLower());
            if (user == null)
            {
                genericResponse.Message = "User does not exist";
                return genericResponse;
            }
            user.Name = $"{onboardRequestPayload.FirstName} {onboardRequestPayload.LastName}";
            user.OnBoardingStage = 1;
            genericResponse.Message = "updated succesfully";
            genericResponse.IsSuccess = true;
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

