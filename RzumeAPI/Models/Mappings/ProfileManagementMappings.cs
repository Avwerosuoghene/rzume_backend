using RzumeAPI.Models.Requests;

namespace RzumeAPI.Models.Mappings;

    public static class ProfileManagementMappings
    {
        private static readonly string ProfileManagementBaseUrl = "api/v2/user/";
        public static readonly Dictionary<string, Type> Mappings = new()
        {
            { $"{ProfileManagementBaseUrl}upload", typeof(OnboardUserRequest) },
        };
    }
