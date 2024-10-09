
using Microsoft.AspNetCore.Mvc;
using System.Net;
using RzumeAPI.Models;
using Microsoft.Extensions.Options;
using RzumeAPI.Options;
using RzumeAPI.Models.Responses;
using RzumeAPI.Models.Requests;
using RzumeAPI.Services.IServices;


namespace RzumeAPI.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController(
        IUserService userService,
        ILogger<UserController> logger
        ) : Controller
    {
        private readonly IUserService _userService = userService;

        private readonly ILogger<UserController> _logger = logger;


        protected APIResponse _response = new();

        private IActionResult HandleResponse(APIServiceResponse<ResultObject> response)
        {
            if (!response.IsSuccess)
            {
                _logger.LogWarning("Operation failed: {@Response}", response.ErrorMessages);
                return StatusCode((int)response.StatusCode, response);
            }
            _logger.LogInformation("Operation successful: {@Response}", response);
            return Ok(response);
        }

        private string GetClientSideBaseUrl(IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {
            return baseUrls.Value.ClientBaseUrl;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest model, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {
            _logger.LogInformation("Register method called with model: {@Request}", model);
            var response = await _userService.RegisterUserWithEmail(model, GetClientSideBaseUrl(baseUrls));
            return HandleResponse(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            _logger.LogInformation("Login method called with model: {@Request}", model);
            var response = await _userService.Login(model);
            return HandleResponse(response);
        }


        [HttpGet("active-user")]
        public async Task<IActionResult> GetActiveUser()
        {
            _logger.LogInformation("GetActiveUser method called");
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            var response = await _userService.GetActiveUser(token);
            return HandleResponse(response);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest model)
        {
            _logger.LogInformation("Logout method called with model: {@Request}", model);
            var response = await _userService.Logout(model);
            return HandleResponse(response);
        }


        [HttpGet("generate-email-token")]
        public async Task<IActionResult> GenerateUserEmailToken([FromQuery] string email, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {
            _logger.LogInformation("GenerateEmailToken method called for email: {Email}", email);
            var response = await _userService.SendUserEmailToken(email, GetClientSideBaseUrl(baseUrls));
            return HandleResponse(response);
        }


        [HttpGet("validate-user-account")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            _logger.LogInformation("ConfirmEmail method called with token: {Token}", token);
            var response = await _userService.ActivateUserAccount(token);
            return HandleResponse(response);
        }




        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleResponse([FromBody] GoogleLoginRequest model)
        {
            _logger.LogInformation("Google Signin method called with token: {@Request}", model);
            var response = await _userService.Login(model);
            return HandleResponse(response);
        }



        [HttpGet("Health")]

        public IActionResult HealthCheck()
        {
            _logger.LogInformation("HealthCheck method called");
            var response = new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new ResultObject
                {
                    Message = "Server running!",
                    Content = null
                }
            };
            return Ok(response);
        }


    }


}

