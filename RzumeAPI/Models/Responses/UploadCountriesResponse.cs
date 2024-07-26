using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Models.Responses;


public class UploadCountriesResponse
{
    public bool IsSuccess { get; set; }

    public  List<CountryDTO>? ExistingCountries { get; set; }


    public string ConvertExistingCountriesToJson()
    {
        string json = JsonConvert.SerializeObject(ExistingCountries, Formatting.None);
        string output = Regex.Replace(json, @"\\", "");
        return output;
    }


}