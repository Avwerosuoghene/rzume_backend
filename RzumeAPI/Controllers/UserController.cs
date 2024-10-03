
using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using Microsoft.Extensions.Options;
using RzumeAPI.Options;
using RzumeAPI.Models.Responses;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Models.Requests;
using AutoMapper;
using Google.Apis.Auth;
using RzumeAPI.Services.IServices;


namespace RzumeAPI.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController(
        IUserRepository userRepo,
        IEmailService emailService,
        IOtpRepository otpRepo,
        IUserService userService,
        ILogger<UserController> logger,
        ITokenService tokenService,
        IMapper mapper,
        IConfiguration configuration
        ) : Controller
    {

        private readonly IUserRepository _userRepo = userRepo;

        private readonly IUserService _userService = userService;

        private readonly string _clientId = configuration["Authentication:Google:ClientId"];



        private readonly ILogger<UserController> _logger = logger;

        private readonly ITokenService _tokenService = tokenService;

        private readonly IMapper _mapper = mapper;



        protected APIResponse _response = new();



  
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest model, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {
            _logger.LogInformation("Register method called with model: {@Request}", model);

            var _baseUrls = baseUrls.Value;
            string clientSideBaseUrl = _baseUrls.ClientBaseUrl;

            var response = await _userService.RegisterUserWithEmail(model, clientSideBaseUrl);

            if (!response.IsSuccess)
            {
                _logger.LogWarning("Registration failed. Response: {@Response}", response.ErrorMessages[0]);
                return StatusCode((int)response.StatusCode, response);
            }

            _logger.LogInformation("User registered successfully. Response: {@Response}", response);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {

            _logger.LogInformation("Login method called with model: {@Request}", model);

            try
            {
                LoginResponse loginResponse = await _userService.Login(model);

                if (loginResponse.User == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(loginResponse.Message);
                    _logger.LogWarning("Login failed due to {response}", loginResponse.Message);
                    return BadRequest(_response);
                }

                if (!loginResponse.EmailConfirmed)
                {
                    loginResponse.User = null;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = new ResultObject
                    {
                        Message = "Kindly Validate your Account",
                        Content = loginResponse
                    };
                    _logger.LogInformation("User email not confirmed. Response: {@Response}", _response);

                    return Ok(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                _response.Result = new ResultObject
                {
                    Message = "Login Successful",
                    Content = loginResponse
                };

                _logger.LogInformation("User logged in successfully. Response: {@Response}", _response);

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during login");
            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);


        }


        [HttpGet("active-user")]
        public async Task<IActionResult> GetActiveUser()
        {
            _logger.LogInformation("GetActiveUser method called");

            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token == null)
                {


                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Invalid Request");
                    _logger.LogWarning("Authorization token is missing");

                    return BadRequest(_response);
                }
                GetActiveUserResponse response = await _userService.GetActiveUser(token);
                if (response.User == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(response.Message);
                    _logger.LogWarning("User not found. Response: {@Response}", _response);

                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "User Retrieved Succesfully",
                    Content = response
                };
                _logger.LogInformation("Active user retrieved successfully. Response: {@Response}", _response);
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving active user");

            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest model)
        {
            _logger.LogInformation("Logout method called with model: {@Request}", model);

            try
            {
                var logoutSuccess = await _userService.Logout(model);

                if (logoutSuccess != true)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Something went wrong");
                    _logger.LogWarning("Logout failed. Response: {@Response}", _response);

                    return BadRequest(_response);

                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "Logout Successful",
                    Content = null
                };
                _logger.LogInformation("User logged out successfully. Response: {@Response}", _response);

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while logging user out");

            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }



        [HttpGet("generate-email-token")]
        public async Task<IActionResult> GenerateEmailToken([FromQuery] string Email, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {
            var _baseUrls = baseUrls.Value;
            string clientSideBaseUrl = _baseUrls.ClientBaseUrl;
            try
            {

                var user = await _userRepo.GetUserByEmailAsync(Email);

                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(UserStatMsg.NotFound);
                    return BadRequest(_response);
                }

                if (user.EmailConfirmed)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(UserStatMsg.EmailValidated);
                    return BadRequest(_response);
                }

                string validateMessage = await _tokenService.SendTokenEmailValidation(user, clientSideBaseUrl);
                if (validateMessage == TokenStatMsg.NotFound)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(TokenStatMsg.NotFound);
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = validateMessage,
                    Content = ""
                };

                return Ok(_response);

            }
            catch (Exception ex)
            {
                {
                    Console.WriteLine(ex);
                }

                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }


        }


        [HttpGet("validate-user-account")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            try
            {
                if (token == null)
                {


                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Invalid Request");
                    return BadRequest(_response);
                }


                ActivateUserAccountResponse activateAccountReponse = await _userService.ActivateUserAccount(token);

                if (activateAccountReponse.User == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(activateAccountReponse.Message);
                    return BadRequest(_response);
                }




                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = UserStatMsg.AccountActivated,
                    Content = activateAccountReponse
                };
                return Ok(_response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }



        }



        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleResponse([FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls, [FromBody] GoogleLoginRequest model)
        {
            var clientId = _clientId;
            _logger.LogInformation("Google Signin method called with token: {@Request}", model);
            var payload = await GoogleJsonWebSignature.ValidateAsync(model.UserToken, new GoogleJsonWebSignature.ValidationSettings
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

                return BadRequest("Failed to retrieve user information.");
            }

            _logger.LogInformation("User mail obtained from claim with value: {@Request}", userInfo.Email);

            GoogleSigninRequest requestModel = new()
            {
                Email = userInfo.Email,
                Name = userInfo.Name,
                GivenName = userInfo.GivenName,
                FamilyName = userInfo.FamilyName
            };

            var loginResponse = await _userService.Login(requestModel);

            if (loginResponse.User == null)
            {
                GoogleSignupResponse response = await _userService.RegisterUserWithGoogle(requestModel);
            
                if (response.User == null)
                {
                    string responseMsg = response.Message ?? "Error while registering";
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(responseMsg);
                    _logger.LogError("Error while registering user. Response: {@Response}", _response);
                    return BadRequest(_response);
                }
                string token = await _tokenService.GenerateToken(response.User, DateTime.UtcNow.AddHours(5), TokenTypes.Login);
                loginResponse = new()
                {
                    Token = token,
                    User = _mapper.Map<UserDTO>(response.User),
                    EmailConfirmed = true,
                };
                _logger.LogInformation("User registered successfully. Response: {@Response}", _response);


            }


            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;

            _response.Result = new ResultObject
            {
                Message = "Login Successful",
                Content = loginResponse
            };

            _logger.LogInformation("User logged in successfully. Response: {@Response}", _response);

            return Ok(_response);



        }



        [HttpGet("Health")]

        public IActionResult HealthCheck()
        {
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = new ResultObject
            {
                Message = "Server running!",
                Content = null
            };


            return Ok(_response);
        }


    }


}

