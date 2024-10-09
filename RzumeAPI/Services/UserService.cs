using System.Net;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Repository;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Services.IServices;

namespace RzumeAPI.Services
{
    public class UserService(
        ITokenService tokenService,
        IUserRepository userRepo,
        SignInManager<User> signInManager,
        ILogger<UserRepository> logger,
        UserManager<User> userManager,
        IEmailService emailService,
        IMapper mapper,
        IOtpService otpService,
        IOtpRepository otpRepository,
        IUtilityService utilityService,
          IConfiguration configuration
        ) : IUserService
    {


        private readonly IUserRepository _userRepo = userRepo;

        private readonly ITokenService _tokenService = tokenService;

        private readonly ILogger<UserRepository> _logger = logger;

        private readonly IOtpService _otpService = otpService;
        private readonly IUtilityService _utilityService = utilityService;


        private readonly UserManager<User> _userManager = userManager;

        private readonly IEmailService _emailService = emailService;

        private readonly IMapper _mapper = mapper;

        private readonly IOtpRepository _otpRepository = otpRepository;


        private readonly SignInManager<User> _signInManager = signInManager;

        private readonly string _clientId = configuration["Authentication:Google:ClientId"];










        private async Task<string> CheckUserMailStatus(string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);

            if (user == null)
            {
                return UserStatMsg.NotFound;
            }


            return user.EmailConfirmed ? "User already exists and email is confirmed" : "User exists but email is not confirmed.";
        }











        public async Task<APIServiceResponse<ResultObject>> ActivateUserAccount(string token)
        {
            _logger.LogInformation("Starting ActivateUserAccount with token: {Token}", token);

            try
            {
                GetUserFromTokenResponse response = await _tokenService.GetUserFromToken(token);
                User? user = response.User;

                if (user == null)
                {
                    _logger.LogWarning("User not found for token: {Token}", token);


                    return new APIServiceResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = [response.Message]
                    };
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogInformation("User {Email} already confirmed", user.Email);


                    return new APIServiceResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = [UserStatMsg.EmailValidated]
                    };
                }

                _logger.LogInformation("Removing old token and confirming email for user {Email}", user.Email);
                await _userManager.RemoveAuthenticationTokenAsync(user, TokenTypes.SignUp, $"{TokenTypes.SignUp}");
                string loginToken = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.Login);

                user.EmailConfirmed = true;
                await _userRepo.UpdateAsync(user);

                _logger.LogInformation("User {Email} account activated successfully", user.Email);


                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = new ResultObject
                    {
                        Message = UserStatMsg.AccountActivated,
                        Content = new ActivateUserAccountResponse
                        {
                            User = _mapper.Map<UserDTO>(user),
                            Token = loginToken
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during ActivateUserAccount for token: {Token}", token);


                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = ["An error occurred"]
                };
            }
        }


        public async Task<RegisterUserResponse<ResultObject>> RegisterUserWithEmail(RegistrationRequest model, string clientSideBaseUrl)
        {
            _logger.LogInformation("RegisterUser method called with model: {@Request}", model);
            string userEmailStat = await CheckUserMailStatus(model.Email);

            if (userEmailStat != UserStatMsg.NotFound)
            {
                _logger.LogWarning("User signup failed with error: {userEmailStat}", userEmailStat);

                return new RegisterUserResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.Conflict,
                    IsSuccess = false,
                    ErrorMessages = [userEmailStat]
                };

            }


            User user = new()
            {
                Email = model.Email,
                Name = string.Empty,
                NormalizedEmail = model.Email.ToUpper(),
                UserName = model.Email,
                TwoFactorEnabled = true,
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            try
            {
                if (result.Succeeded)
                {
                    string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.SignUp);

                    _logger.LogInformation("User registered successfully.");
                    return new RegisterUserResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSuccess = true,
                        Result = new ResultObject
                        {
                            Message = "Kindly check your mail for the confirmation token",
                            Content = user
                        }
                    };
                }
                else
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception($"{ErrorMsgs.RegistrationFailed}: {errorMessages}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for {Email}", user.Email);

                string responseMsg = ex.Message ?? "Error while registering";
                return new RegisterUserResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages =
                    [
                    responseMsg
                    ]

                };
            }


        }



        public async Task<GoogleSignupResponse> RegisterUserWithGoogle(GoogleSigninRequest googleRequest)
        {
            _logger.LogInformation("RegisterUser method called with model: {@Request}", googleRequest);
            User user = new()
            {
                Email = googleRequest.Email,
                Name = googleRequest.Name,
                FirstName = googleRequest.GivenName,
                LastName = googleRequest.FamilyName,
                NormalizedEmail = googleRequest.Email.ToUpper(),
                UserName = googleRequest.GivenName,
                TwoFactorEnabled = true,
                EmailConfirmed = true
            };
            string defaultPassword = _utilityService.GenerateDefaultPassword();
            IdentityResult result = await _userManager.CreateAsync(user, defaultPassword);

            try
            {
                if (result.Succeeded)
                {
                    _logger.LogInformation("Google user {Email} created successfully", user.Email);

                    var userToReturn = await _userRepo.GetUserByEmailAsync(user.Email);
                    if (userToReturn != null)
                    {
                        return new GoogleSignupResponse
                        {
                            User = userToReturn,
                        };
                    }
                    else
                    {
                        _logger.LogError("User retrieval failed for {Email}", user.Email);
                        throw new Exception(ErrorMsgs.UserRetrievalError);
                    }
                }
                else
                {
                    var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Google registration failed for {Email}: {Errors}", user.Email, errorMessages);
                    throw new Exception($"{ErrorMsgs.RegistrationFailed}: {errorMessages}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during HandleGoogleSignup for {Email}", user.Email);
                return new GoogleSignupResponse
                {
                    User = null,
                    Message = ex.Message
                };
            }


        }
        public async Task<APIServiceResponse<ResultObject>> GetActiveUser(string token)
        {
            _logger.LogInformation("Starting GetActiveUser with token: {Token}", token);
            if (token == null)
            {
                _logger.LogWarning("Authorization token is missing");

                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = ["Invalid Request"]
                };
            }

            try
            {

                GetUserFromTokenResponse response = await _tokenService.GetUserFromToken(token);

                if (response.User == null)
                {
                    _logger.LogWarning("No active user found for token: {Token}", token);

                    return new APIServiceResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = [response.Message]
                    };


                }
                _logger.LogInformation("Active user retrieved: {Email}", response.User.Email);
                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = new ResultObject
                    {
                        Message = SuccessMsg.Default,
                        Content = new GetActiveUserResponse
                        {

                            User = _mapper.Map<UserDTO>(response.User)
                        }
                    }
                };




            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GetActiveUser for token: {Token}", token);


                string responseMsg = ex.Message ?? "Error while registering";
                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages =
                    [
                    responseMsg
                    ]

                };

            }

        }


        public async Task<APIServiceResponse<ResultObject>> Login(object loginRequest)
        {

            try
            {
                if (loginRequest is LoginRequest emailRequest)
                {
                    return await HandleEmailLogin(emailRequest);




                }
                else if (loginRequest is GoogleLoginRequest googleRequest)
                {
                    return await HandleGoogleLogin(googleRequest);
                }
                else
                {
                    throw new Exception(ErrorMsgs.InvalidRegReq);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Logging in");


                string responseMsg = ex.Message ?? "Error during login";
                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages =
                    [
                    responseMsg
                    ]

                };

            }




        }

        private async Task<APIServiceResponse<ResultObject>> HandleEmailLogin(LoginRequest emailRequest)
        {
            _logger.LogInformation("Handling email login for {Email}", emailRequest.Email);

            var user = await _userRepo.GetUserByEmailAsync(emailRequest.Email);

            if (user == null)
            {
                _logger.LogWarning("User not found for email {Email}", emailRequest.Email);


                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = [UserStatMsg.InvalidDetails]
                };
            }

            _logger.LogInformation("User found for email {Email}. Checking password.", emailRequest.Email);
            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, emailRequest.Password, lockoutOnFailure: true);



            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning("User with email {Email} is locked out", emailRequest.Email);

                    return new APIServiceResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = [UserStatMsg.LockedOut]
                    };
                };
                _logger.LogWarning("Sign-in failed for email {Email}", emailRequest.Email);


                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = [UserStatMsg.InvalidDetails]
                };


            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email not confirmed for user {Email}", emailRequest.Email);



                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = new ResultObject
                    {
                        Message = UserStatMsg.EmailNotConfirmedMsg
                    }
                };
            }

            _logger.LogInformation("Generating token for user {Email}", emailRequest.Email);
            string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.Login);

            _logger.LogInformation("Login successful for email {Email}", emailRequest.Email);



            return new APIServiceResponse<ResultObject>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new ResultObject
                {
                    Message = "Login Successful",
                    Content = new LoginResponseContent
                    {
                        Token = token,
                        User = _mapper.Map<UserDTO>(user)
                    }
                }
            };
        }


        private async Task<APIServiceResponse<ResultObject>> HandleGoogleLogin(GoogleLoginRequest googleRequest)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleRequest.UserToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            });

            var userInfo = new UserInfo
            {
                Email = payload.Email,
                Name = payload.Name,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName,
                PictureUrl = payload.Picture,
                Locale = payload.Locale
            };

            if (userInfo.Email == null || userInfo.Name == null)
            {
                _logger.LogWarning("Failed to retrieve user infomation from userToken");


                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = ["Failed to retrieve user information."]
                };
            }

            _logger.LogInformation("User mail obtained from claim with value: {@Request}", userInfo.Email);

            GoogleSigninRequest requestModel = new()
            {
                Email = userInfo.Email,
                Name = userInfo.Name,
                GivenName = userInfo.GivenName,
                FamilyName = userInfo.FamilyName
            };



            var user = await _userRepo.GetUserByEmailAsync(requestModel.Email);



            if (user == null)
            {
                GoogleSignupResponse googleSignupResponse = await RegisterUserWithGoogle(requestModel);
                if (googleSignupResponse.User == null)
                {
                    _logger.LogError("Error while registering user");

                    string responseMsg = googleSignupResponse.Message ?? "Error while registering";

                    return new APIServiceResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = [responseMsg]
                    };
                }

            }


            string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.Login);



            LoginResponse loginResponse = new()
            {
                Token = token,
                User = _mapper.Map<UserDTO>(user),
                EmailConfirmed = true,


            };

            return new APIServiceResponse<ResultObject>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new ResultObject
                {
                    Message = "Login Successful",
                    Content = loginResponse
                }
            };


        }





        public async Task<APIServiceResponse<ResultObject>> Logout(LogoutRequest logoutRequestDTO)
        {
            _logger.LogInformation("Logout method called with model: {@Request}", logoutRequestDTO);

            try
            {
                var user = await _userRepo.GetUserByEmailAsync(logoutRequestDTO.Email);

                if (user == null)
                {
                    _logger.LogWarning("Logout failed");


                    return new APIServiceResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = ["Something went wrong"]
                    };
                }
                await _userManager.RemoveAuthenticationTokenAsync(user, TokenTypes.Login, $"{TokenTypes.Login}");

                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = new ResultObject
                    {
                        Message = "Logout Successful",
                        Content = null
                    }
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Logging in");


                string responseMsg = ex.Message ?? "Error during logout";
                return new APIServiceResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages =
                    [
                    responseMsg
                    ]

                };

            }
        }

        public async Task<OtpPasswordResetRequestResponseDTO> InitiateOtpResetPassword(OtpPasswordResetRequestDTO passwordResetRequestModel)
        {
            var user = await _userRepo.GetUserByEmailAsync(passwordResetRequestModel.Email);
            OtpPasswordResetRequestResponseDTO passwordResetResponse = new OtpPasswordResetRequestResponseDTO();
            if (user == null)
            {
                passwordResetResponse.IsSuccess = false;
                passwordResetResponse.Message = UserStatMsg.NotFound;
                return passwordResetResponse;
            }

            var otpModel = await _otpRepository.GetAsync(u => u.UserId == user.Id);

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


    }
}