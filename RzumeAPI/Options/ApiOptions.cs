namespace RzumeAPI.Options;

public class ApiOptions
{
    public const string SectionName = "ApiSettings";
    public string Secret { get; set; } = string.Empty;
}