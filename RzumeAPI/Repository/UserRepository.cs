
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        UserManager<User> userManager, IMapper mapper, IEmailRepository emailService, IOtpRepository dbOtp, TokenService tokenService, UserService userService, SignInManager<User> signInManager, ILogger<UserRepository> logger) : IUserRepository
    {
        private ApplicationDbContext _db = db;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IEmailRepository _emailService = emailService;
        private readonly IMapper _mapper = mapper;

        private readonly UserService _userService = userService;


        private readonly IOtpRepository _dbOtp = dbOtp;

        private readonly OtpService _otpService = otpService;

        private readonly TokenService _tokenService = tokenService;
        private readonly ILogger<UserRepository> _logger = logger;


        public async Task<string> SendTokenEmailValidation(User user, string clientSideBaseUrl)
        {
            _logger.LogInformation("Starting SendTokenEmailValidation for user {Email}", user.Email);

            var activeToken = await _userManager.GetAuthenticationTokenAsync(user, TokenTypes.SignUp, $"{TokenTypes.SignUp}");
            if (activeToken != null)
            {
                _logger.LogInformation("Existing token found for user {Email}", user.Email);

                TokenServiceResponse tokenServiceResponse = _tokenService.ValidateToken(activeToken);
                if (tokenServiceResponse.Message != TokenStatMsg.TokenExpired)
                {
                    _logger.LogInformation("Token is still active for user {Email}", user.Email);
                    return TokenStatMsg.ActivationTokenActive;
                }
            }

            _logger.LogInformation("Generating new token for user {Email}", user.Email);
            string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.SignUp);

            await GenerateMail(user, TokenTypes.SignUp, true, clientSideBaseUrl, token);

            _logger.LogInformation("Token sent via email for user {Email}", user.Email);
            return TokenStatMsg.ActivationTokenSent;
        }


        public async Task<ActivateUserAccountResponse> ActivateUserAccount(string token)
        {
            _logger.LogInformation("Starting ActivateUserAccount with token: {Token}", token);

            try
            {
                GetUserFromTokenResponse response = await _userService.GetUserFromToken(token);
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
                await UpdateAsync(user);

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


        public async Task<RegisterUserResponse> Register(object registrationDTO, string? clientSideBaseUrl)
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



        private async Task<RegisterUserResponse> HandleEmailSignup(RegistrationRequest emailRequest, string clientBaseUrl)
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

                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.Email == user.Email);
                    if (userToReturn != null)
                    {
                        _logger.LogInformation("User {Email} retrieved from database", user.Email);

                        string token = await _tokenService.GenerateToken(user, DateTime.UtcNow.AddHours(5), TokenTypes.SignUp);
                        await GenerateMail(userToReturn, TokenTypes.SignUp, true, clientBaseUrl, token);

                        return new RegisterUserResponse
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
                return new RegisterUserResponse
                {
                    User = null,
                    Message = ex.Message
                };
            }
        }
        private async Task<RegisterUserResponse> HandleGoogleSignup(GoogleSigninRequest googleRequest)
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

            string defaultPassword = _userService.GenerateDefaultPassword();
            IdentityResult result = await _userManager.CreateAsync(user, defaultPassword);

            try
            {
                if (result.Succeeded)
                {
                    _logger.LogInformation("Google user {Email} created successfully", user.Email);

                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.Email == user.Email);
                    if (userToReturn != null)
                    {
                        return new RegisterUserResponse
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
                return new RegisterUserResponse
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

                GetUserFromTokenResponse response = await _userService.GetUserFromToken(token);

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

        // private async Task<GetUserFromTokenResponse> GetUserFromToken(string token)
        // {
        //     TokenServiceResponse tokenServiceResponse = _tokenService.ValidateToken(token);
        //     string? userMail = tokenServiceResponse.UserMail;
        //     if (userMail == null)
        //     {
        //         return new GetUserFromTokenResponse()
        //         {
        //             User = null,
        //             Message = tokenServiceResponse.Message
        //         };
        //     }

        //     if (tokenServiceResponse.Message == TokenStatMsg.TokenExpired)
        //     {


        //         return new GetUserFromTokenResponse()
        //         {
        //             User = null,
        //             Message = TokenStatMsg.ActivationTokenActive
        //         };
        //     }

        //     var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == userMail.ToLower());
        //     if (user == null)
        //     {
        //         return new GetUserFromTokenResponse()
        //         {
        //             User = null,
        //             Message = UserStatMsg.NotFound
        //         };
        //     }

        //     return new GetUserFromTokenResponse()
        //     {
        //         User = user,
        //         Message = UserStatMsg.Found
        //     };
        // }


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

            var user = _db.ApplicationUsers
                .Where(u => u.Email != null)
                .FirstOrDefault(u => u.Email!.ToLower() == emailRequest.Email.ToLower());

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
            var user = _db.ApplicationUsers.Where(u => u.Email != null).FirstOrDefault(u => u.Email!.ToLower() == googleRequest.Email.ToLower());



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

            var user = _db.ApplicationUsers
    .Where(u => u.Email != null)
    .FirstOrDefault(u => u.Email.ToLower() == logoutRequestDTO.Email.ToLower());

            if (user == null)
            {
                return false;
            }
            await _userManager.RemoveAuthenticationTokenAsync(user, TokenTypes.Login, $"{TokenTypes.Login}");
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

