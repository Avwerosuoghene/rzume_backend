using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using RzumeAPI.Helpers;
using RzumeAPI.Services;



namespace RzumeAPI.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UtilityController : Controller
    {

        protected APIResponse _response;

        private readonly IUtilityRepository _utilityRepo;




        public UtilityController( IUtilityRepository utilityRepository)
        {
            _utilityRepo = utilityRepository;
            _response = new();
       
        }



        [HttpPost("update-country-list")]
        public async Task<IActionResult> UpdateCountryList(UploadCountryRequestDTO countryList)
        {
            try
            {
                if (countryList == null)
                {



                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Please provide a valid list"));

                }

                UploadCountriesResponseDTO uploadRequestResponse = await _utilityRepo.UpdateCountryList(countryList);






                if (uploadRequestResponse == null)
                {
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                }

                List<CountryDTO> existingCountries = uploadRequestResponse.ExistingCountries;
                int numberOfExistingCountries = existingCountries.Count;
                if (numberOfExistingCountries > 0)
                {
                    string discoveredCountriesJson = uploadRequestResponse.ConvertExistingCountriesToJson();

                    ResultObject resultObject = new()
                    {
                        Content = existingCountries,
                        Message = $"{numberOfExistingCountries} existing countr{(numberOfExistingCountries == 1 ? "y" : "ies")} discovered"
                    };
                    return BadRequest(ApiResponseFactory.GenerateBadRequest($"The request contains existing countries ", resultObject));

                }
                if (uploadRequestResponse.IsSuccess == false)
                {
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));


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
                return BadRequest(ApiResponseFactory.GenerateBadRequest(ex.Message));

            }
        }


        [HttpGet("update-country-list")]
        public async Task<IActionResult> GetCountryList()
        {
            try
            {
                List<CountryDTO> getCountriesResponse = await _utilityRepo.GetCountryList();

                if (getCountriesResponse == null)
                {
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));

                }

                GetCountryResponseDTO obtainedCountryList = new()
                {
                    CountryList = getCountriesResponse
                };


                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "Countries returned succesfully",
                    Content = obtainedCountryList
                };
                return Ok(_response);


            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.GenerateBadRequest(ex.Message));

            }

        }








    }



}
