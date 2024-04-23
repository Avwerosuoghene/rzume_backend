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
using Newtonsoft.Json;


namespace RzumeAPI.Controllers
{

    [Route("api/v{version:apiVersion}/UsersProfile")]
    [ApiController]
    [ApiVersionNeutral]
    public class ProfileManagementController : Controller
    {

        private readonly IProfileRepository _profileRepo;
        private readonly MiscellaneousHelper _helperService;

        private readonly string _uploadDirectory;

        // private readonly IEmailRepository _emailRepo;

        // private readonly IOtpRepository _otpRepo;


        //Marking this as protected makes it accessible to the parent class
        //and any other class that inherits from this parent class
        protected APIResponse _response;

        public ProfileManagementController(IProfileRepository profileRepo, MiscellaneousHelper helperService, IOtpRepository otpRepo)
        {
            _profileRepo = profileRepo;
            _response = new();
            _helperService = helperService;

            _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }



        // [HttpPost("file-upload")]

        // public async Task<IActionResult> UploadFile(OnboardUserSecondStageRequestDTO fileModel)
        // {


        //     try
        //     {
        //         string path = Path.Combine("", fileModel.FileName)
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //     }

        //     _response.StatusCode = HttpStatusCode.InternalServerError;
        //     _response.IsSuccess = false;
        //     _response.ErrorMessages.Add("An error occured");
        //     return BadRequest(_response);
        // }


        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            Console.WriteLine(file);

            // Generate a unique file name
            string uniqueFileName = Path.Combine(_uploadDirectory, file.FileName);

            // Save the uploaded file to the server
            using (var fileStream = new FileStream(uniqueFileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Ok("File uploaded successfully");
        }




        [HttpPost("user-onboarding")]

        public async Task<IActionResult> OnboardUser(OnboardUserRequestDTO onboardUserPayload)
        {
            GenericResponseDTO? response = null;

            try
            {
                if (onboardUserPayload.OnboardUserPayload == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Please provide onboarding stage payload");
                    return BadRequest(_response);
                }
                if (onboardUserPayload.OnBoardingStage == 0)
                {

                    OnboardUserFirstStageRequestDTO onboardUserFirstStagePayload = JsonConvert.DeserializeObject<OnboardUserFirstStageRequestDTO>(onboardUserPayload.OnboardUserPayload.ToString());
                    response = await _profileRepo.OnboardingFirstStage(onboardUserFirstStagePayload, onboardUserPayload.UserMail);


                }
                if (onboardUserPayload.OnBoardingStage == 1)
                {
                    OnboardUserSecondStageRequestDTO onboardUserSecondStagePayload = JsonConvert.DeserializeObject<OnboardUserSecondStageRequestDTO>(onboardUserPayload.OnboardUserPayload.ToString());
                    response = await _profileRepo.OnboardingSecondStage(onboardUserSecondStagePayload, onboardUserPayload.UserMail);

                }
                if (onboardUserPayload.OnBoardingStage == 2) {
                    
                }

                if (response == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Bad Request");
                    return BadRequest(_response);
                }
                if (response.isSuccess == false)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(response.message);
                    return BadRequest(_response);
                }


                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = response.message,
                    Content = response.isSuccess
                };
                return Ok(_response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return BadRequest(_response);
            }



        }


    }


}

