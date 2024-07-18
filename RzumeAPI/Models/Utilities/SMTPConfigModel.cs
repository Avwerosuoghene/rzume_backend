

namespace RzumeAPI.Models.Utilities;

public class SMTPConfigModel
{
    public required string SenderAddress { get; set; }
    public required string SenderDisplayName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSSL { get; set; }
    public bool UserDefaulCredentials { get; set; }
    public bool IsBodyHTML { get; set; }



}
