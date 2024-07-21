using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace RzumeAPI.Middleware;

public class PayloadValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDictionary<string, Type> _payloadTypeMappings;

    public PayloadValidationMiddleware(RequestDelegate next, IDictionary<string, Type> payloadTypeMappings)
    {
        _next = next;
        _payloadTypeMappings = payloadTypeMappings;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        
        // Determine the route or other criteria to select the payload type
        var requestPath = context.Request.Path.Value.ToLower();
        Type payloadType;

        if (_payloadTypeMappings.TryGetValue(requestPath, out payloadType))
        {
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
                {
                    string requestBody;
                    using (var reader = new System.IO.StreamReader(context.Request.Body))
                    {
                        requestBody = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0; // Reset the request body stream position
                    }

                    // Check if the request body is valid JSON
                    if (!IsValidJson(requestBody))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Invalid JSON payload.");
                        return;
                    }

                    // Deserialize the payload to JObject for validation
                    var jsonObject = JObject.Parse(requestBody);

                    // Call the payload validation method
                    if (!CheckOnboardPayloadValidity(payloadType, jsonObject))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Payload validation failed.");
                        return;
                    }
                }
            }
        }

        await _next(context);
    }

    private bool IsValidJson(string json)
    {
        try
        {
            JObject.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private bool CheckOnboardPayloadValidity(Type payloadType, JObject stringedPayload)
    {
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