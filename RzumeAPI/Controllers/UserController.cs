using System;
using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using RzumeAPI.Repository;

namespace RzumeAPI.Controllers
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepo;

        private readonly IEmailRepository _emailRepo;

        //Marking this as protected makes it accessible to the parent class
        //and any other class that inherits from this parent class
        protected APIResponse _response;

        public UserController(IUserRepository userRepo, IEmailRepository emailRepository)
        {
            _userRepo = userRepo;
            _response = new();
            _emailRepo = emailRepository;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegistrationDTO model)
        {
            bool userNameIsUnique = _userRepo.IsUniqueUser(model.Email);

            if (!userNameIsUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }


            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

         [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model) {

            var loginResponse = await _userRepo.Login(model);

              if(loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
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




        [HttpGet("Health")]

        public async Task<IActionResult> GetUser()
        {
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Sever Running";

            UserEmailOptions options = new UserEmailOptions {
                ToEmails = new List<string>{"kesuion1@gmail.com"},
                Placeholders = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("{{userName}}", "Avwerosuoghene")
                }
                
            };

            await _emailRepo.SendTestEmail(options);
            return Ok(_response);
        }
    }


}

