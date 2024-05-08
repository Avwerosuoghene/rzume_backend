using RzumeAPI.Models.DTO;

namespace RzumeAPI.Repository.IRepository
{
    public interface IUtilityRepository
    {

       
        Task<UploadCountriesResponseDTO> UpdateCountryList(UploadCountryRequestDTO updateCountryListPayload);
         Task<GenericResponseDTO> GetCountryList();

    };
}


