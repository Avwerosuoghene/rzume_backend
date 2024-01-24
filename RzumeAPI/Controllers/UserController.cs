using System;
using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using RzumeAPI.Repository;
using RzumeAPI.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace RzumeAPI.Controllers
{

    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepo;
        private readonly MiscellaneousHelper _helperService;

        private readonly IEmailRepository _emailRepo;

        private readonly IOtpRepository _otpRepo;


        //Marking this as protected makes it accessible to the parent class
        //and any other class that inherits from this parent class
        protected APIResponse _response;

        public UserController(IUserRepository userRepo, IEmailRepository emailRepository, IConfiguration configuration, MiscellaneousHelper helperService, IOtpRepository otpRepo)
        {
            _userRepo = userRepo;
            _response = new();
            _emailRepo = emailRepository;
            _helperService = helperService;
            _otpRepo = otpRepo;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegistrationDTO model)
        {
            bool userNameIsUnique = _helperService.IsUniqueUser(model.Email);
            // bool userNameIsUnique = true;

            if (!userNameIsUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var response = await _userRepo.Register(model);
            if (response == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
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

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {

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
                    await GenerateToken(otpPayload);
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
                if (!otpPasswordResetResponse.isSuccess)
                {

                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(otpPasswordResetResponse.message);
                    return BadRequest(_response);




                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = otpPasswordResetResponse.message!,
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
        public async Task<IActionResult> GenerateToken(GenerateOtpDTO otpPayload)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(otpPayload.Email);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("User not found");
                    return BadRequest(_response);
                }


                var otpModel = await _otpRepo.GetAsync(u => u.UserId == user.Id);


                var token = MiscellaneousHelper.GenerateOtp();
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


                await _emailRepo.SendConfrirmationMail(user, otpResponse.OtpValue, otpPayload.Purpose, false);

                GenerateOtpResponseDTO otpGenerateResponse = new GenerateOtpResponseDTO{
                    isSuccess = true
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

        [HttpPost("confirm-user")]
        public async Task<IActionResult> ConfirmEmail(OtpValidationDTO otpValidationPayload)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(otpValidationPayload.Email);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("User not found");
                    return BadRequest(_response);
                }

                if (user.EmailConfirmed)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Requested user already validated");
                    return BadRequest(_response);
                }

                OtpValidationResponseDTO otpConfirmedResponse = await _helperService.ConfirmOtp(user, otpValidationPayload.OtpValue.ToString()!);

                if (!otpConfirmedResponse.isValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(otpConfirmedResponse.message);
                    return BadRequest(_response);
                }

                user.EmailConfirmed = true;

                User updatedUser = await _userRepo.UpdateAsync(user);

                LoginRequestDTO loginPayload = new LoginRequestDTO
                {
                    UserName = otpValidationPayload.Email,
                    Password = otpValidationPayload.Password
                };

                var loginResponse = await _userRepo.Login(loginPayload);
                if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username or password is incorrect");
                    return BadRequest(_response);
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
                    _response.ErrorMessages.Add("User not found");
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

