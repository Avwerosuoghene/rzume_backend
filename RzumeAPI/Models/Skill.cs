using System.Collections.ObjectModel;

namespace RzumeAPI.Models
{
    public class Skill
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Collection<User> Users { get; set; } = [];
}
}

