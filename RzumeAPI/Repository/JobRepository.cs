
using AutoMapper;

using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Repository.IRepository;
using RzumeAPI.Models.Responses;
using RzumeAPI.Services;


namespace RzumeAPI.Repository
{
    public class JobRepository(ApplicationDbContext db,
  IMapper mapper, IUserRepository userRepo, UserService userService) : IJobRepository
    {
        private readonly ApplicationDbContext _db = db;

        private readonly IUserRepository _userRepo = userRepo;

        private readonly UserService _userService = userService;

        private readonly IMapper _mapper = mapper;


        public async Task<User> UpdateAsync(User user)
        {



            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();
            return user;
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

