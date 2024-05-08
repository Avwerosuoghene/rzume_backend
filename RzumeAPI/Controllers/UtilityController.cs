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

    [Route("api/v{version:apiVersion}/Utility")]
    [ApiController]
    [ApiVersionNeutral]
    public class UtilityController : Controller
    {

        protected APIResponse _response;

        private readonly IUtilityRepository _utilityRepo;


        public UtilityController(MiscellaneousHelper helperService, IUtilityRepository utilityRepository)
        {
            _utilityRepo = utilityRepository;
        }



        [HttpPost("update-country-list")]
        public async Task<IActionResult> UpdateCountryList(UploadCountryRequestDTO countryList)
        {
            try
            {
                if (countryList == null)
                {



                    return BadRequest(MiscellaneousHelper.GenerateBadRequest("Please provide a valid list"));

                }

                UploadCountriesResponseDTO uploadRequestResponse = await _utilityRepo.UpdateCountryList(countryList);






                if (uploadRequestResponse == null)
                {
                    return BadRequest(MiscellaneousHelper.GenerateBadRequest("Bad Request"));
                }

                if (uploadRequestResponse.ExistingCountries.Count > 0)
                {
                    string discoveredCountriesJson = uploadRequestResponse.ConvertExistingCountriesToJson();
                    return BadRequest(MiscellaneousHelper.GenerateBadRequest($"The following countries already exists on the db {discoveredCountriesJson}"));

                }
                if (uploadRequestResponse.IsSuccess == false)
                {
                    return BadRequest(MiscellaneousHelper.GenerateBadRequest("Bad Request"));


                }





                GenericContentVal content = new()
                {
                    IsSuccess = true
                };

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "Success",
                    Content = content
                };
                return Ok(_response);

            }
            catch (Exception ex)
            {
                return BadRequest(MiscellaneousHelper.GenerateBadRequest(ex.Message));

            }
        }




        // [HttpPost("user-onboarding")]

        // public async Task<IActionResult> OnboardUser(OnboardUserRequestDTO onboardUserPayload)
        // {


        // }






    }



}
