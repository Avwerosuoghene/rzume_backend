using System;
namespace RzumeAPI.Models.DTO
{
    public class UploadCountryRequestDTO
    {

        public List<CountryDTO> CountryList {get; set;}


    }

    public class GetCountryResponseDTO
    {
        public List<Country> CountryList { get; set; }
    }



}