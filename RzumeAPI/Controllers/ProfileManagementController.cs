﻿using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using RzumeAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RzumeAPI.Models.Utilities;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;
using Microsoft.Extensions.Options;
using RzumeAPI.Options;
using RzumeAPI.Services.IServices;

namespace RzumeAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class ProfileManagementController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly string _uploadDirectory;
        protected APIResponse _response;
        private readonly ILogger<ProfileManagementController> _logger;

        public ProfileManagementController(IProfileService profileService, IOtpRepository otpRepo, ILogger<ProfileManagementController> logger)
        {
            _profileService = profileService;
            _response = new();
            _logger = logger;
            _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload attempt with no file.");
                return BadRequest("No file uploaded");
            }

            _logger.LogInformation("File upload started. File: {FileName}", file.FileName);

            // Generate a unique file name
            string uniqueFileName = Path.Combine(_uploadDirectory, file.FileName);

            // Save the uploaded file to the server
            using (var fileStream = new FileStream(uniqueFileName, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            _logger.LogInformation("File uploaded successfully. File path: {FilePath}", uniqueFileName);
            return Ok("File uploaded successfully");
        }

        [HttpPost("user-onboarding")]
        public async Task<IActionResult> OnboardUser(OnboardUserRequest onboardUserPayload)
        {
            try
            {
                _logger.LogInformation("Onboarding user. Payload: {@Payload}", onboardUserPayload);

                if (onboardUserPayload.OnboardUserPayload == null)
                {
                    _logger.LogWarning("Onboarding attempt with null payload.");
                    return BadRequest(ApiResponseFactory.GenerateBadRequest(ErrorMsgs.InavlidPayload));
                }

                JObject payloadObject = JObject.Parse(onboardUserPayload.OnboardUserPayload.ToString());
                bool validProperties = true;

                GenericResponse response;
                switch (onboardUserPayload.OnBoardingStage)
                {
                    case 0:
                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserFirstStageRequest>(payloadObject);
                        if (!validProperties)
                        {
                            _logger.LogWarning("Onboarding first stage failed. Invalid payload.");
                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                        }

                        OnboardUserFirstStageRequest onboardUserFirstStagePayload = JsonConvert.DeserializeObject<OnboardUserFirstStageRequest>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileService.OnboardingFirstStage(onboardUserFirstStagePayload, onboardUserPayload.Token);
                        break;

                    case 1:
                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserSecondStageRequest>(payloadObject);
                        if (!validProperties)
                        {
                            _logger.LogWarning("Onboarding second stage failed. Invalid payload.");
                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                        }

                        OnboardUserSecondStageRequest onboardUserSecondStagePayload = JsonConvert.DeserializeObject<OnboardUserSecondStageRequest>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileService.OnboardingSecondStage(onboardUserSecondStagePayload, onboardUserPayload.Token);
                        break;

                    case 2:
                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserThirdStageRequest>(payloadObject);
                        if (!validProperties)
                        {
                            _logger.LogWarning("Onboarding third stage failed. Invalid payload.");
                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                        }

                        OnboardUserThirdStageRequest onboardUserThirdStagePayload = JsonConvert.DeserializeObject<OnboardUserThirdStageRequest>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileService.OnboardingThirdStage(onboardUserThirdStagePayload, onboardUserPayload.Token);
                        break;

                    case 3:
                        validProperties = PayloadValidator.CheckOnboardPayloadValidaty<OnboardUserFourthStageRequest>(payloadObject);
                        if (!validProperties)
                        {
                            _logger.LogWarning("Onboarding fourth stage failed. Invalid payload.");
                            return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                        }

                        OnboardUserFourthStageRequest onboardUserFourthStagePayload = JsonConvert.DeserializeObject<OnboardUserFourthStageRequest>(onboardUserPayload.OnboardUserPayload.ToString());
                        response = await _profileService.OnboardingFourthStage(onboardUserFourthStagePayload, onboardUserPayload.Token);
                        break;

                    default:
                        _logger.LogWarning("Invalid onboarding stage: {Stage}", onboardUserPayload.OnBoardingStage);
                        return BadRequest(ApiResponseFactory.GenerateBadRequest("Invalid onboarding stage"));
                }

                if (response == null)
                {
                    _logger.LogWarning("Onboarding process returned null response.");
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                }

                if (response.IsSuccess == false)
                {
                    _logger.LogWarning("Onboarding process failed. Message: {Message}", response.Message);
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
                _logger.LogInformation("Onboarding process completed successfully. Message: {Message}", response.Message);
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the onboarding process.");
                return BadRequest(ApiResponseFactory.GenerateBadRequest(ex.Message));
            }
        }



        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(RequestPasswordReset requestPasswordReset, [FromServices] IOptionsSnapshot<BaseUrlOptions> baseUrls)
        {
            _logger.LogInformation("Passowrd reset request made with email: {@Request}", requestPasswordReset);
            var _baseUrls = baseUrls.Value;
            string clientSideBaseUrl = _baseUrls.ClientBaseUrl;

            try
            {

                var response = await _profileService.RequestPasswordReset(requestPasswordReset, clientSideBaseUrl);

                if (response.IsSuccess == false)
                {
                    _logger.LogWarning("Password reset initiation failed Message: {Message}", response.Message);
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
                _logger.LogInformation("Password reset request initiated succesfully. Message: {Message}", response.Message);
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during password reset request");
            }
            return BadRequest(ApiResponseFactory.GenerateBadRequest("Exception occurred during password reset request"));


        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            _logger.LogInformation("Passowrd reset initiated with email: {@Request}", resetPassword.Email);


            try
            {

                GenericResponse passwordResetResponse = await _profileService.ResetPassword(resetPassword);

                if (passwordResetResponse.IsSuccess == false)
                {
                    _logger.LogWarning("Password reset failed Message: {Message}", passwordResetResponse.Message);
                    return BadRequest(ApiResponseFactory.GenerateBadRequest(passwordResetResponse.Message));
                }


                GenericContentVal content = new()
                {
                    IsSuccess = true
                };

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = passwordResetResponse.Message,
                    Content = content
                };
                _logger.LogInformation("Password reset succesful. Message: {Message}", passwordResetResponse.Message);
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during password reset");
            }
            return BadRequest(ApiResponseFactory.GenerateBadRequest("Exception occurred during password reset"));
        }

        

        

    }



}