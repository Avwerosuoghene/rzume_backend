using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class UserFileDTO
    {
        public string FileId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public string FileCategory { get; set; } = string.Empty;

        public string FileBytes { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
    }


}

