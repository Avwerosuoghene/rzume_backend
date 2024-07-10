using System.Net;
using System.Reflection;
using Newtonsoft.Json.Linq;
using RzumeAPI.Models;

namespace RzumeAPI.Helpers
{


    public class ApiResponseFactory
    {

        protected static APIResponse? _response;

    public ApiResponseFactory(APIResponse response) {
        _response = response;
    }

      

        public static APIResponse GenerateBadRequest(string message, ResultObject? result = null)
        {
            if (result != null) {
                _response!.Result = result;
            }
            _response!.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add(message);
            return _response;
        }

    }
}