
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
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


        public async Task<UploadCountriesResponseDTO> UpdateCountryList(UploadCountryRequestDTO updateCountryListPayload)
        {




            List<CountryDTO> countriesAlreadyExisting = new();
            List<Country> countriesToSave = new();


            List<Country> countryModelList = new();

            updateCountryListPayload.CountryList.ForEach((CountryDTO countryItem) =>
         {


             var countryItemReturned = _db.Country.FirstOrDefault(countryInDb => countryInDb.Name.ToLower() == countryItem.Name.ToLower());
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

            UploadCountriesResponseDTO uploadContryRequestResponse = new();
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


        private GenericResponseDTO GenerateErrorResponse(string message)
        {
            return new GenericResponseDTO
            {
                IsSuccess = false,
                Message = message
            };
        }

        private GenericResponseDTO GenerateSuccessResponse(string message)
        {
            return new GenericResponseDTO
            {
                IsSuccess = true,
                Message = message
            };
        }



    }




}

