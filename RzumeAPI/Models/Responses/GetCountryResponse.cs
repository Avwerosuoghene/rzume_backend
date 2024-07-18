using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models.Responses;


    public class GetCountryResponse
    {
        public List<CountryDTO> CountryList { get; set; } = [];
    }
