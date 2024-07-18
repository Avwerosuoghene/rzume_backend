
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Models.Requests;
using RzumeAPI.Models.Responses;
using RzumeAPI.Repository.IRepository;


namespace RzumeAPI.Repository
{
    public class UtilityRepository : IUtilityRepository
    {
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        public UtilityRepository(ApplicationDbContext db,
  IMapper mapper)
        {
            _db = db;


            _mapper = mapper;






        }


        public async Task<UploadCountriesResponse> UpdateCountryList(UploadCountryRequest updateCountryListPayload)
        {




            List<CountryDTO> countriesAlreadyExisting = [];
            List<Country> countriesToSave = [];


            List<Country> countryModelList = [];

            updateCountryListPayload.CountryList.ForEach((CountryDTO countryItem) =>
         {


             var countryItemReturned = _db.Country.FirstOrDefault(countryInDb => countryInDb.Name.Equals(countryItem.Name, StringComparison.CurrentCultureIgnoreCase));
             if (countryItemReturned != null)
             {
                 CountryDTO countryDTOModel = _mapper.Map<CountryDTO>(countryItem);
                 countriesAlreadyExisting.Add(countryDTOModel);

             }
             else
             {
                 Country countryModel = _mapper.Map<Country>(countryItem);

                 countriesToSave.Add(countryModel);
             }

         });

            UploadCountriesResponse uploadContryRequestResponse = new();
            if (countriesAlreadyExisting.Count > 0)
            {
                uploadContryRequestResponse.IsSuccess = false;
                uploadContryRequestResponse.ExistingCountries = countriesAlreadyExisting;
            }
            else
            {

                uploadContryRequestResponse.IsSuccess = true;
                uploadContryRequestResponse.ExistingCountries = countriesAlreadyExisting;

                _db.Country.AddRange(countriesToSave);
                await _db.SaveChangesAsync();

            }

            return uploadContryRequestResponse;


        }



        public async Task<List<CountryDTO>> GetCountryList()
        {
            var countryModelReturned = await _db.Country.ToListAsync();

            List<CountryDTO> countriesDTO = new();




            foreach (var country in countryModelReturned)
            {
                CountryDTO countryItem = _mapper.Map<CountryDTO>(country);
                countriesDTO.Add(countryItem);

            }

            return countriesDTO;


        }











        // private async Task<List<CountryDTO>> BulkUpdateAsync(List<CountryDTO> countryListPayload)
        // {

        // }


        private GenericResponse GenerateErrorResponse(string message)
        {
            return new GenericResponse
            {
                IsSuccess = false,
                Message = message
            };
        }

        private GenericResponse GenerateSuccessResponse(string message)
        {
            return new GenericResponse
            {
                IsSuccess = true,
                Message = message
            };
        }



    }




}

