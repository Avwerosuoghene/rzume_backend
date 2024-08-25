namespace RzumeAPI.Options;

public class ApiOptions
{
    public const string SectionName = "JwtConfig";
    public string Secret { get; set; } = string.Empty;
}