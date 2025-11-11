using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface IEntityLinkingService
{
    Task<Result<bool>> LinkEntityToUserAsync(Guid entityId, string userId, string role);
}