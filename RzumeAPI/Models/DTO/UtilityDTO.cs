using System;
namespace RzumeAPI.Models.DTO
{
    public class UploadCountryRequestDTO
    {

        public List<CountryDTO> CountryList {get; set;} = new List<CountryDTO>();


    }

    public class GetCountryResponseDTO
    {
        public List<CountryDTO> CountryList { get; set; } = new List<CountryDTO>();
    }



}