using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace RzumeAPI.Models.DTO
{
    public class CountryDTO
    {







        public string Name { get; set; }

        public string Code { get; set; }



    }



    public class UploadCountriesResponseDTO
    {
        public bool IsSuccess { get; set; }

        public List<CountryDTO> ExistingCountries { get; set; }


        public string ConvertExistingCountriesToJson()
        {
            return JsonConvert.SerializeObject(ExistingCountries);
        }


    }
}

