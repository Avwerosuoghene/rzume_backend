using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using RzumeAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace RzumeAPI.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class ProfileManagementController : Controller
    {

        private readonly IProfileRepository _profileRepo;

        private readonly string _uploadDirectory;

     
        protected APIResponse _response;

        public ProfileManagementController(IProfileRepository profileRepo,IOtpRepository otpRepo)
        {
            _profileRepo = profileRepo;
            _response = new();
        

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
            try
            {
                if (onboardUserPayload.OnboardUserPayload == null)
                {



                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Please provide onboarding stage payload"));

                }
                JObject payloadObject = JObject.Parse(onboardUserPayload.OnboardUserPayload.ToString());
                bool validProperties = true;

                GenericResponseDTO? response;
                switch (onboardUserPayload.OnBoardingStage)
                {
                    case 0:
                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserFirstStageRequestDTO>(payloadObject);

                        if (!validProperties)
                        {
                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));

                        }

                        OnboardUserFirstStageRequestDTO onboardUserFirstStagePayload = JsonConvert.DeserializeObject<OnboardUserFirstStageRequestDTO>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileRepo.OnboardingFirstStage(onboardUserFirstStagePayload, onboardUserPayload.UserMail);
                        break;
                    case 1:
                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserSecondStageRequestDTO>(payloadObject);
                        if (!validProperties)
                        {
                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));

                        }

                        OnboardUserSecondStageRequestDTO onboardUserSecondStagePayload = JsonConvert.DeserializeObject<OnboardUserSecondStageRequestDTO>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileRepo.OnboardingSecondStage(onboardUserSecondStagePayload, onboardUserPayload.UserMail);

                        break;
                    case 2:
                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserThirdStageRequestDTO>(payloadObject);
                        if (!validProperties)
                        {
                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));

                        }

                        OnboardUserThirdStageRequestDTO onboardUserThirdStagePayload = JsonConvert.DeserializeObject<OnboardUserThirdStageRequestDTO>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileRepo.OnboardingThirdStage(onboardUserThirdStagePayload, onboardUserPayload.UserMail);
                        break;
                    case 3:

                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserFourthStageRequestDTO>(payloadObject);
                        if (!validProperties)
                        {

                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                        }

                        OnboardUserFourthStageRequestDTO onboardUserFourthStagePayload = JsonConvert.DeserializeObject<OnboardUserFourthStageRequestDTO>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileRepo.OnboardingFourthStage(onboardUserFourthStagePayload, onboardUserPayload.UserMail);
                        break;

                    default:

                        return BadRequest(ApiResponseFactory.GenerateBadRequest("Invalid onboarding stage"));

                }




                if (response == null)
                {
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                }
                if (response.IsSuccess == false)
                {


                    return BadRequest(ApiResponseFactory.GenerateBadRequest(response.Message));
                }

                GenericContentVal content = new()
                {
                    IsSuccess = true
                };

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = response.Message,
                    Content = content
                };
                return Ok(_response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);



                return BadRequest(ApiResponseFactory.GenerateBadRequest(ex.Message));

            }



        }






    }


}

