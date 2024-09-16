
namespace RzumeAPI.Services.IServices;
public interface IFileService
{
    Task<string> WriteFile(IFormFile file);
}