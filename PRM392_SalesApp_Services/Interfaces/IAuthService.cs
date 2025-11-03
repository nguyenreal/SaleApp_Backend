
using PRM392.SalesApp.Repositories.Models;
using PRM392.SalesApp.Services.DTOs;

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}