﻿
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Services;

namespace RzumeAPI.Repository
{
    public class UserRepository(ApplicationDbContext db, OtpService otpService,
        UserManager<User> userManager, IMapper mapper, IEmailRepository emailService, IOtpRepository dbOtp, TokenService tokenService) : IUserRepository
    {
        private ApplicationDbContext _db = db;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IEmailRepository _emailService = emailService;
        private readonly IMapper _mapper = mapper;


        private readonly IOtpRepository _dbOtp = dbOtp;

        private readonly OtpService _otpService = otpService;

        private readonly TokenService _tokenService = tokenService;

        public async Task<string> SendTokenEmailValidation(User user, string clientSideBaseUrl)
        {


            var activeToken = await _userManager.GetAuthenticationTokenAsync(user, TokenTypes.SignUp, $"{TokenTypes.SignUp}Token");
            if (activeToken != null)
            {
                TokenServiceResponse tokenServiceResponse = _tokenService.ValidateToken(activeToken);
                if (tokenServiceResponse.Message != TokenStatMsg.TokenExpired)
                {
                    return TokenStatMsg.ActivationTokenActive;
                }

            }

            string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.SignUp);


            await GenerateMail(user, TokenTypes.SignUp, true, clientSideBaseUrl, token);


            return TokenStatMsg.ActivationTokenSent;

        }

        public async Task<ActivateUserAccountResponse> ActivateUserAccount(string token)
        {

            try
            {
                GetUserFromTokenResponse response = await GetUserFromToken(token);

                User? user = response.User;

                if (user == null)
                {
                    return new ActivateUserAccountResponse()
                    {

                        User = null,
                        Message = response.Message

                    };
                }

                if (user.EmailConfirmed)
                {
                    return new ActivateUserAccountResponse()
                    {


                        User = null,
                        Message = UserStatMsg.EmailValidated

                    };
                }

                string loginToken = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.Login);

                user.EmailConfirmed = true;
                await UpdateAsync(user);

                return new ActivateUserAccountResponse()
                {

                    User = _mapper.Map<UserDTO>(user),
                    Token = loginToken

                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ActivateUserAccountResponse()
                {
                    User = null,
                    Message = "An error occurred"
                };
            }


        }



        public async Task<RegisterUserResponse> Register(RegistrationRequest registrationDTO, string clientSideBaseUrl)
        {
            User user = new()
            {
                Email = registrationDTO.Email,
                Name = string.Empty,
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
                        string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.SignUp);

                        await GenerateMail(userToReturn, TokenTypes.SignUp, true, clientSideBaseUrl, token);

                        UserDTO returnedUser = _mapper.Map<UserDTO>(userToReturn);
                        return new RegisterUserResponse()
                        {
                            User = returnedUser,
                        };
                    }
                    else
                    {
                        throw new Exception(ErrorMsgs.UserRetrievalError);

                    }



                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        // Log or handle each error as needed
                        Console.WriteLine($"Error: {error.Description}");
                    }

                    throw new Exception(ErrorMsgs.RegistrationFailed);
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

                GetUserFromTokenResponse response = await GetUserFromToken(token);

                if (response.User == null)
                {
                    return new GetActiveUserResponse()
                    {


                        User = null,
                        Message = response.Message

                    };
                }



                return new GetActiveUserResponse()
                {


                    User = _mapper.Map<UserDTO>(response.User),
                    Message = SuccessMsg.Default

                };



            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new GetActiveUserResponse()
                {
                    User = null,
                    Message = ErrorMsgs.Default
                };
            }

        }

        private async Task<GetUserFromTokenResponse> GetUserFromToken(string token)
        {
            TokenServiceResponse tokenServiceResponse = _tokenService.ValidateToken(token);
            string? userMail = tokenServiceResponse.UserMail;
            if (userMail == null)
            {
                return new GetUserFromTokenResponse()
                {
                    User = null,
                    Message = tokenServiceResponse.Message
                };
            }

            if (tokenServiceResponse.Message == TokenStatMsg.TokenExpired)
            {


                return new GetUserFromTokenResponse()
                {
                    User = null,
                    Message = TokenStatMsg.ActivationTokenActive
                };
            }

            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == userMail.ToLower());
            if (user == null)
            {
                return new GetUserFromTokenResponse()
                {
                    User = null,
                    Message = UserStatMsg.NotFound
                };
            }

            return new GetUserFromTokenResponse()
            {
                User = user,
                Message = UserStatMsg.Found
            };
        }


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private async Task GenerateMail(User user, string mailPurpose, bool isSigin, string clientBaseUrl, string token)
        {


            string linkPath = $"{clientBaseUrl}auth/email-confirmation?token={token}";
            string templatePath = @"EmailTemplate/{0}.html";
            string mailSubject = "Kindly click the link to validate your email.";
            string templateName = "EmailConfirm";
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








        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {

            // The async version of this method is FirstOrDefaultAsync
            var user = _db.ApplicationUsers.Where(u => u.Email != null).FirstOrDefault(u => u.Email!.ToLower() == loginRequest.Email.ToLower());
            bool isValid = false;

            if (user != null)
            {
                isValid = await _userManager.CheckPasswordAsync(user, loginRequest!.Password);

            }

            if (user == null || isValid == false)
            {
                return new LoginResponse()
                {
                    Token = "",
                    User = null,
                    Message = UserStatMsg.InvalidDetails
                };
            }

            if (!user.EmailConfirmed)
            {
                return new LoginResponse()
                {
                    Token = "",
                    User = _mapper.Map<UserDTO>(user),
                    EmailConfirmed = false,
                    Message = UserStatMsg.EmailNotConfirmedMsg
                };
            }

            string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.Login);



            LoginResponse loginResponseDTO = new()
            {
                Token = token,
                User = _mapper.Map<UserDTO>(user),
                EmailConfirmed = true,


            };


            return loginResponseDTO;

        }


        public async Task<bool> Logout(LogoutRequest logoutRequestDTO)
        {
            // var user = _db.ApplicationUsers.FirstOrDefault(u =>  u.Email?.ToLower() ?? u.Email?.ToLower() == logoutRequestDTO.Email.ToLower());

            var user = _db.ApplicationUsers
    .Where(u => u.Email != null)
    .FirstOrDefault(u => u.Email.ToLower() == logoutRequestDTO.Email.ToLower());

            if (user == null)
            {
                return false;
            }
            await _userManager.RemoveAuthenticationTokenAsync(user, TokenTypes.Login, $"{TokenTypes.Login}Token");
            return true;

        }

        public async Task<OtpPasswordResetRequestResponseDTO> InitiateOtpResetPassword(OtpPasswordResetRequestDTO passwordResetRequestModel)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email!.ToLower() == passwordResetRequestModel.Email.ToLower());
            OtpPasswordResetRequestResponseDTO passwordResetResponse = new OtpPasswordResetRequestResponseDTO();
            if (user == null)
            {
                passwordResetResponse.IsSuccess = false;
                passwordResetResponse.Message = UserStatMsg.NotFound;
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
                passwordResetResponse.Message = PasswordStatMsgs.SuccesfulReset;
                return passwordResetResponse;
            }
            else
            {
                passwordResetResponse.IsSuccess = false;
                passwordResetResponse.Message = PasswordStatMsgs.FailedReset;
                return passwordResetResponse;
            }





        }



        public async Task<User> UpdateAsync(User user)
        {



            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }




}

