namespace RzumeAPI.Models.Requests;

 public class OnboardUserSecondStageRequest
    {

        public required string FileName { get; set; }  
        public required string FileBytes { get; set; } 

        public required string FileCat {get; set;} 

    }