using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using RzumeAPI.Data;
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Helpers
{


    public class MiscellaneousHelper
    {
        private string secretKey;

        private IOtpRepository _dbOtp;


        private ApplicationDbContext _db;

        public MiscellaneousHelper(ApplicationDbContext db, IConfiguration configuration, IOtpRepository dbOtp)
        {
            _db = db;

            _dbOtp = dbOtp;


            secretKey = configuration.GetValue<string>("ApiSettings:Secret");

        }

        public static string GenerateOtp()
        {
            Random generator = new Random();
            string generatedOtp = generator.Next(1, 10000).ToString("D4");
            return generatedOtp;

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

        public string GenerateToken(string userId, string userMail)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Describes what our token will contain
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.Email,userMail)
                }),

                //Describes Token Expiration
                Expires = DateTime.UtcNow.AddDays(7),

                //Describes signin creddentials
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var uniqueGenerateToken = tokenHandler.WriteToken(token);
            return uniqueGenerateToken;
        }

        public async Task<string> WriteFile(IFormFile file)
        {
            string filename = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = DateTime.Now.Ticks.ToString() + extension;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files");
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
                using (var stream = new FileStream(exactPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {

            }
            return filename;
        }

        public async Task<OtpValidationResponseDTO> ConfirmOtp(User user, string otpPayloadValue)
        {
            var otpModel = await _dbOtp.GetAsync(u => u.UserId == user.Id);
            OtpValidationResponseDTO responseValue = new OtpValidationResponseDTO();



            if (otpModel.OtpValue.ToString() != otpPayloadValue)
            {


                responseValue.isValid = false;
                responseValue.message = "Invalid otp value";

                return responseValue;
            }


            DateTime currentDate = DateTime.Now;
            if (currentDate > otpModel.ExpirationDate)
            {

                responseValue.isValid = false;
                responseValue.message = "Otp has expired";
                ;
                return responseValue;


            }

            if (otpModel.IsConfirmed)
            {

                responseValue.isValid = false;
                responseValue.message = "Otp validation failed";

                return responseValue;
            }

            otpModel.IsConfirmed = true;

            Otp otpResponse = await _dbOtp.UpdateAsync(otpModel);


            responseValue.isValid = true;
            responseValue.message = "Otp validation succesful";

            return responseValue;

        }

    }
}