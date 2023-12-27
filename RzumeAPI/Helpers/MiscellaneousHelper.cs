using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using RzumeAPI.Data;

namespace RzumeAPI.Helpers
{


    public class MiscellaneousHelper
    {
        private string secretKey;

        private ApplicationDbContext _db;

        public MiscellaneousHelper(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;

            secretKey = configuration.GetValue<string>("ApiSettings:Secret");

        }

        public static int GenerateOtp()
        {
            Random generator = new Random();
            int generatedOtp = generator.Next(1, 1000000);
            string sixDigitOtp = generatedOtp.ToString("D6");
            return int.Parse(sixDigitOtp);

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
    }
}