
using RzumeAPI.Models;
using RzumeAPI.Models.DTO;

namespace RzumeAPI.Repository.IRepository
{
    public interface IExperienceRepository : IRepository<Experience>
    {

        Task<bool> UpdateAsync(Experience education);

        Task<bool> BulkUpdateAsync(List<ExperienceDTO> experienceItems, string userId);
    }
}