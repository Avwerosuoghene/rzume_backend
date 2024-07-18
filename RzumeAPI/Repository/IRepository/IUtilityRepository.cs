using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Responses;
using RzumeAPI.Models.Requests;

namespace RzumeAPI.Repository.IRepository
{
    public interface IUtilityRepository
    {

       
        Task<UploadCountriesResponse> UpdateCountryList(UploadCountryRequest updateCountryListPayload);
        Task<List<CountryDTO>> GetCountryList();

    };
}


