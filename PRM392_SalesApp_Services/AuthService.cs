using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRM392.SalesApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Kiểm tra xem username hoặc email đã tồn tại chưa
            if (await _userRepository.GetByUsernameAsync(registerDto.Username) != null)
            {
                throw new Exception("Username already exists");
            }
            if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
            {
                throw new Exception("Email already exists");
            }

            // 2. Hash mật khẩu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // 3. Tạo user mới
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                Role = "User" // Gán vai trò mặc định
            };

            // 4. Lưu vào DB
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // 1. Tìm user bằng username
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                throw new Exception("Invalid username or password");
            }

            // 2. Kiểm tra mật khẩu
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid username or password");
            }

            // 3. Tạo JWT
            string token = CreateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        private string CreateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Token hết hạn sau 7 ngày
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}