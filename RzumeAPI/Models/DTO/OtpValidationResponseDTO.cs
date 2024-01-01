using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class OtpValidationResponseDTO 
    {

        public bool isValid { get; set; }

        public string message { get; set; }

 
    }
}

