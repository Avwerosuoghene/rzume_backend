
using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using RzumeAPI.Services;
using Microsoft.Extensions.Options;
using RzumeAPI.Options;


namespace RzumeAPI.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepo;
        private readonly OtpService _otpService;

        private readonly IEmailRepository _emailRepo;

        private readonly IOtpRepository _otpRepo;
        private readonly UserService _userService;


        //Marking this as protected makes it accessible to the parent class
        //and any other class that inherits from this parent class
        protected APIResponse _response;

        public UserController(IUserRepository userRepo, IEmailRepository emailRepository, OtpService otpService, IOtpRepository otpRepo, UserService userService)
        {
            _userRepo = userRepo;
            _response = new();
            _emailRepo = emailRepository;
            _otpService = otpService;
            _otpRepo = otpRepo;
            _userService = userService;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegistrationDTO model, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {

            User? user = _userService.userExists(model.Email);
            var _baseUrls = baseUrls.Value;
            string clientSideBaseUrl = _baseUrls.ClientBaseUrl;

            if (user != null && user.EmailConfirmed)
            {
                _response.StatusCode = HttpStatusCode.Conflict;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(UserStatMsg.UserExistsMsg);
                return BadRequest(_response);
            }

            if (user != null && !user.EmailConfirmed)
            {
                _response.StatusCode = HttpStatusCode.Conflict;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(UserStatMsg.EmailNotConfirmedMsg);
                return BadRequest(_response);
            }

            RegisterUserResponse response = await _userRepo.Register(model, clientSideBaseUrl)!;
            if (response.User == null)
            {
                string responseMsg = response.Message ?? "Error while registering";
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(responseMsg);
                return BadRequest(_response);
            }

            RegistrationResponseDTO signupResponse = new RegistrationResponseDTO
            {
                IsCreated = true
            };
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = new ResultObject
            {
                Message = "Kindly check your mail for the confirmation token",
                Content = signupResponse
            };

            return Ok(_response);
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {

            var _baseUrls = baseUrls.Value;
            string clientSideBaseUrl = _baseUrls.ClientBaseUrl;
            try
            {
                var loginResponse = await _userRepo.Login(model);

                if (loginResponse.User == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username or password is incorrect");
                    return BadRequest(_response);
                }

                if (!loginResponse.EmailConfirmed)
                {
                    GenerateOtpDTO otpPayload = new GenerateOtpDTO
                    {
                        Email = loginResponse.User.Email,
                        Purpose = "User Validation"
                    };
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = new ResultObject
                    {
                        Message = "Kindly Validate your Account",
                        Content = loginResponse
                    };
                    return Ok(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                _response.Result = new ResultObject
                {
                    Message = "Login Successful",
                    Content = loginResponse
                };


                return Ok(_response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);


        }


        [HttpGet("active-user")]
        public async Task<IActionResult> GetActiveUser()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token == null)
                {


                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Invalid Request");
                    return BadRequest(_response);
                }
                GetActiveUserResponse response = await _userRepo.GetActiveUser(token);
                if (response.User == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(response.Message);
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "User Retrieved Succesfully",
                    Content = response
                };
                return Ok(_response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO model)
        {

            try
            {
                var logoutSuccess = await _userRepo.Logout(model);

                if (logoutSuccess != true)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Something went wrong");
                    return BadRequest(_response);

                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "Logout Successful",
                    Content = null
                };

                return Ok(_response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }

        [HttpPost("otp-reset-pass")]
        public async Task<IActionResult> InitiateOtpResetPassword([FromBody] OtpPasswordResetRequestDTO model)
        {

            try
            {
                OtpPasswordResetRequestResponseDTO otpPasswordResetResponse = await _userRepo.InitiateOtpResetPassword(model);
                if (!otpPasswordResetResponse.IsSuccess)
                {

                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(otpPasswordResetResponse.Message);
                    return BadRequest(_response);




                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = otpPasswordResetResponse.Message!,
                    Content = otpPasswordResetResponse
                };

                return Ok(_response);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);

        }




        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken(GenerateOtpDTO otpPayload, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {
            var _baseUrls = baseUrls.Value;
            string clientSideBaseUrl = _baseUrls.ClientBaseUrl;
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(otpPayload.Email);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(UserStatMsg.UserNotFound);
                    return BadRequest(_response);
                }


                var otpModel = await _otpRepo.GetAsync(u => u.UserId == user.Id);


                var token = _otpService.GenerateOtp();
                DateTime currentDate = DateTime.Now;
                DateTime expirationDate = currentDate.AddMinutes(5);

                otpModel.OtpValue = token;
                otpModel.ExpirationDate = expirationDate;
                otpModel.IsConfirmed = false;

                Otp otpResponse = await _otpRepo.UpdateAsync(otpModel);

                if (otpResponse == null)
                {
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error occured updating the db");
                    return BadRequest(_response);
                }


                await _emailRepo.SendConfrirmationMail(user, otpResponse.OtpValue, otpPayload.Purpose, false, clientSideBaseUrl);

                GenerateOtpResponseDTO otpGenerateResponse = new GenerateOtpResponseDTO
                {
                    IsSuccess = true
                };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "otp sent succesfully",
                    Content = otpGenerateResponse
                };

                return Ok(_response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(UserStatMsg.UserNotFound);
                    return BadRequest(_response);
                }

                if (user.EmailConfirmed)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(UserStatMsg.EmailValidated);
                    return BadRequest(_response);
                }

                string validateMessage = await _userRepo.SendTokenEmailValidation(user, clientSideBaseUrl);

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


        [HttpPost("validate-user-account")]
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


                ActivateUserAccountResponse activateAccountReponse = await _userRepo.ActivateUserAccount(token);

                if (!activateAccountReponse.AccountActivated)
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
                    Message = activateAccountReponse.Message,
                    Content = null
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


        [HttpPost("validate-email")]
        public async Task<IActionResult> ValidateEmail(ValidateEmailDTO validatePayload)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(validatePayload.Email);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(UserStatMsg.UserNotFound);
                    return BadRequest(_response);
                }

                ValidateEmailResponseDTO validateResponse = new ValidateEmailResponseDTO
                {
                    isValidated = true
                };


                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "Succesful",
                    Content = validateResponse
                };
                return Ok(_response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            return BadRequest(_response);

        }



        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("google-response")] // Specify the route for the action
        public IActionResult GoogleResponse()
        {
            var authenticationResult = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result;

            if (!authenticationResult.Succeeded)
            {
                // Handle failed authentication
                return BadRequest("Failed to authenticate with Google.");
            }

            var claims = authenticationResult.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            return Json(claims);
        }


        [HttpPost("user-onboarding")]







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

