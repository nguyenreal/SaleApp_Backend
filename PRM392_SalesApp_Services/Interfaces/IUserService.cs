using PRM392.SalesApp.Services.DTOs;

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserProfileAsync(int userId);
    }
}