using Microsoft.AspNetCore.Mvc;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;
using System.Net;
using RzumeAPI.Helpers;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;

namespace RzumeAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UtilityController(IUtilityRepository utilityRepository, ILogger<UtilityController> logger) : Controller
    {
        protected APIResponse _response = new();
        private readonly IUtilityRepository _utilityRepo = utilityRepository;
        private readonly ILogger<UtilityController> _logger = logger;

        [HttpPost("update-country-list")]
        public async Task<IActionResult> UpdateCountryList(UploadCountryRequest countryList)
        {
            _logger.LogInformation("UpdateCountryList called with payload: {@CountryList}", countryList);

            try
            {
                if (countryList == null)
                {
                    _logger.LogWarning("UpdateCountryList failed: No payload provided.");
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Please provide a valid list"));
                }

                _logger.LogInformation("Processing country list update.");

                UploadCountriesResponse uploadRequestResponse = await _utilityRepo.UpdateCountryList(countryList);

                if (uploadRequestResponse == null)
                {
                    _logger.LogWarning("UpdateCountryList failed: Response is null.");
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                }

                List<CountryDTO> existingCountries = uploadRequestResponse.ExistingCountries ?? new List<CountryDTO>();
                int numberOfExistingCountries = existingCountries.Count;

                if (numberOfExistingCountries > 0)
                {
                    string discoveredCountriesJson = uploadRequestResponse.ConvertExistingCountriesToJson();
                    _logger.LogInformation("Discovered existing countries: {DiscoveredCountries}", discoveredCountriesJson);

                    ResultObject resultObject = new()
                    {
                        Content = existingCountries,
                        Message = $"{numberOfExistingCountries} existing countr{(numberOfExistingCountries == 1 ? "y" : "ies")} discovered"
                    };
                    return BadRequest(ApiResponseFactory.GenerateBadRequest($"The request contains existing countries", resultObject));
                }

                if (!uploadRequestResponse.IsSuccess)
                {
                    _logger.LogWarning("UpdateCountryList failed: Response indicates failure.");
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
                _logger.LogInformation("Country list updated successfully.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the country list.");
                return BadRequest(ApiResponseFactory.GenerateBadRequest(ex.Message));
            }
        }

        [HttpGet("update-country-list")]
        public async Task<IActionResult> GetCountryList()
        {
            _logger.LogInformation("GetCountryList called.");

            try
            {
                List<CountryDTO> getCountriesResponse = await _utilityRepo.GetCountryList();

                if (getCountriesResponse == null)
                {
                    _logger.LogWarning("GetCountryList failed: Response is null.");
                    return BadRequest(ApiResponseFactory.GenerateBadRequest("Bad Request"));
                }

                GetCountryResponse obtainedCountryList = new()
                {
                    CountryList = getCountriesResponse
                };

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = new ResultObject
                {
                    Message = "Countries returned successfully",
                    Content = obtainedCountryList
                };
                _logger.LogInformation("Country list retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the country list.");
                return BadRequest(ApiResponseFactory.GenerateBadRequest(ex.Message));
            }
        }
    }
}