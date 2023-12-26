using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RzumeAPI.Data;
using RzumeAPI.Helpers;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IEmailRepository _emailService;
        private string secretKey;
        private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        private IOtpRepository _dbOtp;





        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IEmailRepository emailService)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;


        }

        public async Task<UserDTO> Register(RegistrationDTO registrationDTO)
        {
            User user = new()
            {
                Verified = false,
                Email = registrationDTO.Email,
                NormalizedEmail = registrationDTO.Email.ToUpper(),
                UserName = registrationDTO.Email,
                TwoFactorEnabled = true

            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationDTO.Password);

                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers
                .FirstOrDefault(u => u.UserName == registrationDTO.Email);

                    var token = MiscellaneousHelper.GenerateOtp();
                    DateTime currentDate = DateTime.Now;
                    DateTime expirationDate = currentDate.AddMinutes(5);

                    Console.WriteLine(userToReturn.Id);

                
                    OtpDTO otp = new OtpDTO
                    {
                        UserId = userToReturn!.Id.ToString(),
                        ExpirationDate = expirationDate,
                        OtpValue = token.ToString()
                
                    };



                     Otp otpModel = _mapper.Map<Otp>(otp);

                    // await _dbOtp.CreateAsync(otpModel);

                    //  await _dbOtp.AddAsync(otp);
                    await _db.Otp.AddAsync(otpModel);
                    await _db.SaveChangesAsync();


                    await SendEmailConfirmationEmail(user, token);

                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task GenerateEmailConfirmationToken(User user)
        {
            // var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var token = MiscellaneousHelper.GenerateOtp();



            await SendEmailConfirmationEmail(user, token);

        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private async Task SendEmailConfirmationEmail(User user, int token)
        {

            // string appDomain = _configuration.GetValue<string>("Application:AppDomain");
            // string confirmationLink = _configuration.GetValue<string>("Application:EmailConfirmation");


            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string> { user.Email },
                Placeholders = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("{{userName}}", user.UserName),
                    new KeyValuePair<string, string>("{{link}}", token.ToString() )

                }

            };

            await _emailService.SendConfrirmationMail(options);
        }



        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            bool isValid = false;

            if (user != null)
            {
                isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            }

            if (user == null || isValid == false)
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


        public async Task<IdentityResult> ConfirmEmail(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);

        }

    }




}

