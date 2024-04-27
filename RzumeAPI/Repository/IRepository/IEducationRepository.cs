
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Repository.IRepository
{
    public interface IEducationRepository : IRepository<Education>
    {

        Task<bool> UpdateAsync(Education education);

        Task<bool> BulkUpdateAsync(List<EducationDTO> educationItems, string userId);
    }
}