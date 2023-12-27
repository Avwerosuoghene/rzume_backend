using System;
using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using RzumeAPI.Repository;
using RzumeAPI.Helpers;

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


            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = new
            {
                message = "Kindly check your mail for the confirmation token"
            };
            return Ok(_response);
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {

            var loginResponse = await _userRepo.Login(model);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }


        [HttpGet("confirm-email")]
        public async Task ConfirmEmail(string uid, string token, string email)
        {
            EmailConfirm model = new EmailConfirm
            {
                Email = email
            };
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _userRepo.ConfirmEmail(uid, token);
                if (result.Succeeded)
                {
                    Console.WriteLine("yes");
                }
            }
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken(GenerateOtpDTO otpPayload)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(otpPayload.Email);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username not found");
                    return BadRequest(_response);
                }


                var otpModel = await _otpRepo.GetAsync(u => u.UserId == user.Id);

                Otp otpResponse = await _otpRepo.UpdateAsync(otpModel);

                if (otpResponse == null)
                {
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error occured updating the db");
                    return BadRequest(_response);
                }


                await _emailRepo.SendConfrirmationMail(user, otpResponse.OtpValue);


                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new {message = "otp sent succesfully"};
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



        // [HttpPost("confirm-email")]
        // public async Task ConfirmEmail(EmailConfirm model)
        // {
        //     try
        //     {
        //         var user = await _userRepo.GetUserByEmailAsync(model.Email);
        //         if (user != null)
        //         {

        //             if (user.EmailConfirmed)
        //             {
        //                 model.IsConfirmed = true;
        //                 return;
        //             }

        //             await _userRepo.GenerateEmailConfirmationToken(user);
        //             model.EmailSent = true;
        //             ModelState.Clear();

        //         }
        //         else
        //         {

        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //     }

        // }




        [HttpGet("Health")]

        public IActionResult HealthCheck()
        {
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result =  new {
                message = "Server running!"
            };


            return Ok(_response);
        }


    }


}

