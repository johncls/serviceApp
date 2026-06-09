using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceApp.Application.DTOs;
using ServiceApp.Domain.Entities;
using ServiceApp.Domain.Interfaces;

namespace ServiceApp.Application.Services
{
    public class UserLoginService
    {
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IConfiguration _configuration;
        public UserLoginService(IUserLoginRepository userLoginRepository, IConfiguration configuration)
        {
            _userLoginRepository = userLoginRepository;
            _configuration = configuration;
        }


        public async Task<UserLoginResponseDto> LoginUserAsync(string identification, string password)
        {
            var user = await _userLoginRepository.GetByIdAsync(identification, password);
            if (user is null)
            {
                return new UserLoginResponseDto { Success = false, Message = "User not found" };
            };
            return new UserLoginResponseDto { Success = true, Message = "User retrieved successfully", Token = GenerateToken(user) };
        }

        private string GenerateToken(UserLogin user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Identification),
                new Claim(ClaimTypes.Name, user.Name)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}