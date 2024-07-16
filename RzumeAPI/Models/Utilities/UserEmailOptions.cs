

namespace RzumeAPI.Models.Utilities;

public class UserEmailOptions
{
    public List<string> ToEmails { get; set; } = new List<string>();

    public string Body { get; set; }

    public string Subject { get; set; }

    public List<KeyValuePair<string, string>> Placeholders { get; set; } = new List<KeyValuePair<string, string>>();
}
