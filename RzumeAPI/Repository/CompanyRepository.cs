
using AutoMapper;

using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Models.Responses;
using RzumeAPI.Services;
using Microsoft.EntityFrameworkCore;


namespace RzumeAPI.Repository
{
    public class ComapanyRepository(ApplicationDbContext db) : ICompanyRepository
    {
        private readonly ApplicationDbContext _db = db;


        public async Task<Company> UpdateAsync(Company company)
        {



            _db.Company.Update(company);
            await _db.SaveChangesAsync();
            return company;
        }

          public async Task<Company?> FindCompanyByName(string companyName)
        {
            var company = await _db.Company.FirstOrDefaultAsync((c => c.Name == companyName));
            return company;
         
        }



        private static GenericResponse GenerateErrorResponse(string message)
        {
            return new GenericResponse
            {
                IsSuccess = false,
                Message = message
            };
        }

        private static GenericResponse GenerateSuccessResponse(string message)
        {
            return new GenericResponse
            {
                IsSuccess = true,
                Message = message
            };
        }

    }




}

