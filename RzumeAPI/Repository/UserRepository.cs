using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private string secretKey;
        private readonly IMapper _mapper;


        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            _userManager = userManager;
        }

        public bool IsUniqueUser(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return true;
            }

            return false;
        }

        public async Task<UserDTO> Register(RegistrationDTO registrationDTO)
        {
            User user = new()
            {
                Verified = false,
                Email = registrationDTO.Email,
                NormalizedEmail = registrationDTO.Email.ToUpper(),
                UserName = registrationDTO.Email,

            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationDTO.Password);

                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers
                .FirstOrDefault(u => u.UserName == registrationDTO.Email);

                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }



        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

              if(user == null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    //WeakReference couldn't use the token directly as it is of type SecurityToken
                    Token = "",
                    User = null
                };
            }

                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Describes what our token will contain
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email.ToString())
                }),

                //Describes Token Expiration
                Expires = DateTime.UtcNow.AddDays(7),

                //Describes signin creddentials
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

             var token = tokenHandler.CreateToken(tokenDescriptor);


              LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
            };


            return loginResponseDTO;

        }

   
    }




}

