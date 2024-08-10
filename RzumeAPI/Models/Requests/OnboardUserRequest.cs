namespace RzumeAPI.Models.Requests;


public class OnboardUserRequest
    {
        public int OnBoardingStage { get; set; } 

        public dynamic OnboardUserPayload { get; set; } = null!;

        public required string Token { get; set; } 


    }