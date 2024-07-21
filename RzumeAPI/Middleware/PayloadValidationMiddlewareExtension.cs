namespace RzumeAPI.Middleware;


public static class PayloadValidationMiddlewareExtensions
{
    public static IApplicationBuilder UsePayloadValidation(this IApplicationBuilder builder, IDictionary<string, Type> payloadTypeMappings)
    {
        return builder.UseMiddleware<PayloadValidationMiddleware>(payloadTypeMappings);
    }
}