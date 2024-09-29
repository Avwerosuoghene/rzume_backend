using System.Net;
using AutoMapper;
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
        IUtilityService utilityService
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









        public async Task<User?> UserExists(string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            return user;
        }









        public async Task<ActivateUserAccountResponse> ActivateUserAccount(string token)
        {
            _logger.LogInformation("Starting ActivateUserAccount with token: {Token}", token);

            try
            {
                GetUserFromTokenResponse response = await _tokenService.GetUserFromToken(token);
                User? user = response.User;

                if (user == null)
                {
                    _logger.LogWarning("User not found for token: {Token}", token);
                    return new ActivateUserAccountResponse
                    {
                        User = null,
                        Message = response.Message
                    };
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogInformation("User {Email} already confirmed", user.Email);
                    return new ActivateUserAccountResponse
                    {
                        User = null,
                        Message = UserStatMsg.EmailValidated
                    };
                }

                _logger.LogInformation("Removing old token and confirming email for user {Email}", user.Email);
                await _userManager.RemoveAuthenticationTokenAsync(user, TokenTypes.SignUp, $"{TokenTypes.SignUp}");
                string loginToken = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.Login);

                user.EmailConfirmed = true;
                await _userRepo.UpdateAsync(user);

                _logger.LogInformation("User {Email} account activated successfully", user.Email);
                return new ActivateUserAccountResponse
                {
                    User = _mapper.Map<UserDTO>(user),
                    Token = loginToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during ActivateUserAccount for token: {Token}", token);
                return new ActivateUserAccountResponse
                {
                    User = null,
                    Message = "An error occurred"
                };
            }
        }

        public async Task<RegisterUserResponse<ResultObject>> RegisterUser(RegistrationRequest model, string clientSideBaseUrl)
        {
            _logger.LogInformation("RegisterUser method called with model: {@Request}", model);

            User? user = await UserExists(model.Email);

            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    _logger.LogWarning("User already exists and email is confirmed.");


                    return new RegisterUserResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        IsSuccess = false,
                        ErrorMessages =
                        [
                            "User already exists and email is confirmed."
                        ]
                    };
                }

                _logger.LogWarning("User exists but email is not confirmed.");
                return new RegisterUserResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.Conflict,
                    IsSuccess = false,
                    ErrorMessages =
                    [
                        "User exists but email is not confirmed."
                    ]
                };
            }

            ProcessUserResponse processUserResponse = await ProcessUserRegistration(model, clientSideBaseUrl);

            if (processUserResponse.User == null)
            {
                string responseMsg = processUserResponse.Message ?? "Error while registering";
                _logger.LogError("Error while registering user.");
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

            _logger.LogInformation("User registered successfully.");
            return new RegisterUserResponse<ResultObject>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new ResultObject
                {
                    Message = "Kindly check your mail for the confirmation token",
                    Content = processUserResponse
                }
            };
        }

        public async Task<RegisterUserResponse<ResultObject>> RegisterUserWithEmail(RegistrationRequest model, string clientSideBaseUrl)
        {
            _logger.LogInformation("RegisterUser method called with model: {@Request}", model);
            User? userExits = await UserExists(model.Email);

            if (userExits != null)
            {
                if (userExits.EmailConfirmed)
                {
                    _logger.LogWarning("User already exists and email is confirmed.");


                    return new RegisterUserResponse<ResultObject>
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        IsSuccess = false,
                        ErrorMessages =
                        [
                            "User already exists and email is confirmed."
                        ]
                    };
                }

                _logger.LogWarning("User exists but email is not confirmed.");
                return new RegisterUserResponse<ResultObject>
                {
                    StatusCode = HttpStatusCode.Conflict,
                    IsSuccess = false,
                    ErrorMessages =
                    [
                        "User exists but email is not confirmed."
                    ]
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
                    _logger.LogInformation("User {Email} created successfully", user.Email);

                    var userToReturn = await _userRepo.GetUserByEmailAsync(user.Email);
                    if (userToReturn != null)
                    {
                        _logger.LogInformation("User {Email} retrieved from database", user.Email);

                        string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.SignUp);
                        await _emailService.GenerateMail(userToReturn, TokenTypes.SignUp, true, clientSideBaseUrl, token);

                        // return new ProcessUserResponse
                        // {
                        //     User = userToReturn,
                        // };
                        _logger.LogInformation("User registered successfully.");
                        return new RegisterUserResponse<ResultObject>
                        {
                            StatusCode = HttpStatusCode.OK,
                            IsSuccess = true,
                            Result = new ResultObject
                            {
                                Message = "Kindly check your mail for the confirmation token",
                                Content = userToReturn
                            }
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
                    _logger.LogError("Registration failed for {Email}: {Errors}", user.Email, errorMessages);
                    throw new Exception($"{ErrorMsgs.RegistrationFailed}: {errorMessages}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during HandleEmailSignup for {Email}", user.Email);

                string responseMsg = ex.Message ?? "Error while registering";
                _logger.LogError("Error while registering user.");
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



        public async Task<RegisterUserResponse<ResultObject>> RegisterUserWithGoogle(GoogleSigninRequest googleRequest, string clientSideBaseUrl)
        {
                        _logger.LogInformation("RegisterUser method called with model: {@Request}", model);


        }
        public async Task<ProcessUserResponse> ProcessUserRegistration(object registrationDTO, string? clientSideBaseUrl)
        {
            _logger.LogInformation("Starting user registration process");


            if (registrationDTO is RegistrationRequest emailRequest)
            {
                _logger.LogInformation("Handling email signup for {Email}", emailRequest.Email);
                return await HandleEmailSignup(emailRequest, clientSideBaseUrl!);
            }
            else if (registrationDTO is GoogleSigninRequest googleRequest)
            {
                _logger.LogInformation("Handling Google signup for {Email}", googleRequest.Email);
                return await HandleGoogleSignup(googleRequest);
            }
            else
            {
                _logger.LogError("Invalid registration request type");
                throw new Exception(ErrorMsgs.InvalidRegReq);
            }
        }



        private async Task<ProcessUserResponse> HandleEmailSignup(RegistrationRequest emailRequest, string clientBaseUrl)
        {
            _logger.LogInformation("Starting HandleEmailSignup for {Email}", emailRequest.Email);

            User user = new()
            {
                Email = emailRequest.Email,
                Name = string.Empty,
                NormalizedEmail = emailRequest.Email.ToUpper(),
                UserName = emailRequest.Email,
                TwoFactorEnabled = true,
            };

            IdentityResult result = await _userManager.CreateAsync(user, emailRequest.Password);

            try
            {
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} created successfully", user.Email);

                    var userToReturn = await _userRepo.GetUserByEmailAsync(user.Email);
                    if (userToReturn != null)
                    {
                        _logger.LogInformation("User {Email} retrieved from database", user.Email);

                        string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.SignUp);
                        await _emailService.GenerateMail(userToReturn, TokenTypes.SignUp, true, clientBaseUrl, token);

                        return new ProcessUserResponse
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
                    _logger.LogError("Registration failed for {Email}: {Errors}", user.Email, errorMessages);
                    throw new Exception($"{ErrorMsgs.RegistrationFailed}: {errorMessages}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during HandleEmailSignup for {Email}", user.Email);
                return new ProcessUserResponse
                {
                    User = null,
                    Message = ex.Message
                };
            }


        }
        private async Task<ProcessUserResponse> HandleGoogleSignup(GoogleSigninRequest googleRequest)
        {
            _logger.LogInformation("Starting HandleGoogleSignup for {Email}", googleRequest.Email);

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
                        return new ProcessUserResponse
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
                return new ProcessUserResponse
                {
                    User = null,
                    Message = ex.Message
                };
            }
        }
        public async Task<GetActiveUserResponse> GetActiveUser(string token)
        {
            _logger.LogInformation("Starting GetActiveUser with token: {Token}", token);

            try
            {

                GetUserFromTokenResponse response = await _tokenService.GetUserFromToken(token);

                if (response.User == null)
                {
                    _logger.LogWarning("No active user found for token: {Token}", token);

                    return new GetActiveUserResponse()
                    {


                        User = null,
                        Message = response.Message

                    };
                }
                _logger.LogInformation("Active user retrieved: {Email}", response.User.Email);




                return new GetActiveUserResponse()
                {


                    User = _mapper.Map<UserDTO>(response.User),
                    Message = SuccessMsg.Default

                };



            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GetActiveUser for token: {Token}", token);

                return new GetActiveUserResponse()
                {
                    User = null,
                    Message = ErrorMsgs.Default
                };
            }

        }


        public async Task<LoginResponse> Login(object loginRequest)
        {



            if (loginRequest is LoginRequest emailRequest)
            {
                return await HandleEmailLogin(emailRequest);




            }
            else if (loginRequest is GoogleSigninRequest googleRequest)
            {
                return await HandleGoogleLogin(googleRequest);
            }
            else
            {
                throw new Exception(ErrorMsgs.InvalidRegReq);
            }


        }

        private async Task<LoginResponse> HandleEmailLogin(LoginRequest emailRequest)
        {
            _logger.LogInformation("Handling email login for {Email}", emailRequest.Email);

            var user = await _userRepo.GetUserByEmailAsync(emailRequest.Email);

            if (user == null)
            {
                _logger.LogWarning("User not found for email {Email}", emailRequest.Email);
                return new LoginResponse
                {
                    Token = "",
                    User = null,
                    Message = UserStatMsg.InvalidDetails
                };
            }

            _logger.LogInformation("User found for email {Email}. Checking password.", emailRequest.Email);
            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, emailRequest.Password, lockoutOnFailure: true);



            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning("User with email {Email} is locked out", emailRequest.Email);
                    return new LoginResponse
                    {
                        Token = "",
                        User = null,
                        Message = UserStatMsg.LockedOut
                    };
                };
                _logger.LogWarning("Sign-in failed for email {Email}", emailRequest.Email);
                return new LoginResponse
                {
                    Token = "",
                    User = null,
                    Message = UserStatMsg.InvalidDetails
                };


            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email not confirmed for user {Email}", emailRequest.Email);
                return new LoginResponse
                {
                    Token = "",
                    User = _mapper.Map<UserDTO>(user),
                    EmailConfirmed = false,
                    Message = UserStatMsg.EmailNotConfirmedMsg
                };
            }

            _logger.LogInformation("Generating token for user {Email}", emailRequest.Email);
            string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.Login);

            _logger.LogInformation("Login successful for email {Email}", emailRequest.Email);
            return new LoginResponse
            {
                Token = token,
                User = _mapper.Map<UserDTO>(user),
                EmailConfirmed = true
            };
        }


        private async Task<LoginResponse> HandleGoogleLogin(GoogleSigninRequest googleRequest)
        {
            var user = await _userRepo.GetUserByEmailAsync(googleRequest.Email);



            if (user == null)
            {
                return new LoginResponse()
                {
                    Token = "",
                    User = null,
                    Message = UserStatMsg.InvalidDetails
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

            var user = await _userRepo.GetUserByEmailAsync(logoutRequestDTO.Email);

            if (user == null)
            {
                return false;
            }
            await _userManager.RemoveAuthenticationTokenAsync(user, TokenTypes.Login, $"{TokenTypes.Login}");
            return true;

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