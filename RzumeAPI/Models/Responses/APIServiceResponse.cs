


using System.Net;

namespace RzumeAPI.Models.Responses;



public class APIServiceResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> ErrorMessages { get; set; } = [];

    public T? Result { get; set; }


}