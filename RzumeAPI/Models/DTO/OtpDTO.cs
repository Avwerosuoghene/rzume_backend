using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
	public class OtpDTO
	{
      
        public string OtpID { get; set; }

        public string UserId { get; set; }

        public string OtpValue {get; set;}


        public DateTime ExpirationDate { get; set; }
    }
}

