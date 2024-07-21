namespace RzumeAPI.Models.Mappings;

public static class PayloadTypeMappings
{
    public static readonly Dictionary<string, Type> Mappings;

    static PayloadTypeMappings()
    {
        Mappings = [];

        LoadMappings(ProfileManagementMappings.Mappings);
    }

    private static void LoadMappings(Dictionary<string, Type> controllerMappings)
    {
        foreach (var mapping in controllerMappings)
        {
            if (!Mappings.ContainsKey(mapping.Key))
            {
                Mappings.Add(mapping.Key, mapping.Value);
            }
            else
            {
                throw new ArgumentException($"Duplicate endpoint mapping: {mapping.Key}");
            }
        }
    }
}
