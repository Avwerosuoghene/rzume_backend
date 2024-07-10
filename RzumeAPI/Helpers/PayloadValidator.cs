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
                if (!stringedPayload.ContainsKey(property.Name))
                    allPropertiesPresent = false;

            }



            return allPropertiesPresent;
        }
    }
}