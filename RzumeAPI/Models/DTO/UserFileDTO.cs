using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RzumeAPI.Models.DTO
{
    public class UserFileDTO
    {

        public string FileId { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FilePath { get; set; }

        public string UserId { get; set; }
    }


}

