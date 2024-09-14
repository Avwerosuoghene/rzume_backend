
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Services.IServices;
    public interface IOtpService
    {
        string GenerateOtp();

        Task<OtpValidationResponseDTO> ConfirmOtp(User user, string otpPayloadValue);
    }