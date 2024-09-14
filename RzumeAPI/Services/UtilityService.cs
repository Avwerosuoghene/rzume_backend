using RzumeAPI.Services.IServices;

namespace RzumeAPI.Services
{
    public class UtilityService(

        ) : IUtilityService
    {

        public string GenerateDefaultPassword()
        {
            const int passwordLength = 16;
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            var passwordChars = new char[passwordLength];
            for (int i = 0; i < passwordLength; i++)
            {
                passwordChars[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(passwordChars);
        }
    }
}