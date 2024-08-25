using RzumeAPI.Models.Utilities;

namespace RzumeAPI.Models.Requests;


public class AddApplicationRequest
    {
        public int UserId { get; set; } 

        public required string Position { get; set; }
        public DateTime? ApplicationDate { get; set; }

        public required Status Status { get; set; } 
        public string? JobLink { get; set; } 
        public string? ResumeLink { get; set; } 

        public int CompanyID { get; set; }




    }