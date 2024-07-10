using RzumeAPI.Models;
using RzumeAPI.Models.DTO;
using RzumeAPI.Repository.IRepository;

namespace RzumeAPI.Services
{
    public class OtpService
    {
        private IOtpRepository _dbOtp;

        public OtpService(IOtpRepository dbOtp)
        {
            _dbOtp = dbOtp;
        }
        public string GenerateOtp()
        {
            Random generator = new Random();
            string generatedOtp = generator.Next(1, 10000).ToString("D4");
            return generatedOtp;

        }


        public async Task<OtpValidationResponseDTO> ConfirmOtp(User user, string otpPayloadValue)
        {
            var otpModel = await _dbOtp.GetAsync(u => u.UserId == user.Id);
            OtpValidationResponseDTO responseValue = new();



            if (otpModel.OtpValue.ToString() != otpPayloadValue)
            {


                responseValue.IsValid = false;
                responseValue.Message = "Invalid otp value";

                return responseValue;
            }


            DateTime currentDate = DateTime.Now;
            if (currentDate > otpModel.ExpirationDate)
            {

                responseValue.IsValid = false;
                responseValue.Message = "Otp has expired";
                ;
                return responseValue;


            }

            if (otpModel.IsConfirmed)
            {

                responseValue.IsValid = false;
                responseValue.Message = "Otp validation failed";

                return responseValue;
            }

            otpModel.IsConfirmed = true;

            Otp otpResponse = await _dbOtp.UpdateAsync(otpModel);


            responseValue.IsValid = true;
            responseValue.Message = "Otp validation succesful";

            return responseValue;

        }


    }
}