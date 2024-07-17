using System.Net;

using RzumeAPI.Models;

namespace RzumeAPI.Helpers
{


    public class ApiResponseFactory
    {

     



        public static APIResponse GenerateBadRequest(string message, ResultObject? result = null)
        {
             APIResponse response = new();
            if (result != null)
            {
                response!.Result = result;
            }
            response.StatusCode = HttpStatusCode.BadRequest;
            response.IsSuccess = false;
            response.ErrorMessages.Add(message);
            return response;
        }

    }
}