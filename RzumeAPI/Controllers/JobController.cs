using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using RzumeAPI.Models.Requests;

namespace RzumeAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    [Authorize]
    public class JobController(ILogger<ProfileManagementController> logger) : Controller
    {
        protected APIResponse _response = new();
        private readonly ILogger<ProfileManagementController> _logger = logger;

         [HttpPost("create-application")]
            public async Task<IActionResult> AddApplication(AddApplicationRequest addApplicationRequest)
        {
           
            _logger.LogInformation($"Application request initiated from user with id: {addApplicationRequest.UserId}");


            _logger.LogInformation("Job application added succesfully");
            return Ok("Applicatio");
        }

    }



}