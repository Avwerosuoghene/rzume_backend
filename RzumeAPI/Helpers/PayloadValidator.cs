using System.Reflection;
using Newtonsoft.Json.Linq;

namespace RzumeAPI.Helpers
{


    public class PayloadValidator
    {

        public static bool CheckOnboardPayloadValidaty<T>(JObject stringedPayload)
        {



            Type payloadType = typeof(T);

            PropertyInfo[] expectedProperties = payloadType.GetProperties();

            bool allPropertiesPresent = true;

            foreach (var property in expectedProperties)
            {
                bool propertyExists = stringedPayload.Properties()
                    .Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));

                if (!propertyExists)
                {
                    allPropertiesPresent = false;
                    break;
                }
            }



            return allPropertiesPresent;
        }
  
    }
}