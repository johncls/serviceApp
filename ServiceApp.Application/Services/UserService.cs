using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceApp.Application.DTOs;
using ServiceApp.Domain.Entities;
using ServiceApp.Domain.Interfaces;

namespace ServiceApp.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto> CreateUserAsync(UserRequestDto request)
        {
            var user = new User
            {
                _id = Guid.NewGuid().ToString(),
                Identification = request.Identification,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                MessageCount = 0,
                Status = false,
                Message = request.Message,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = request.IsActive
            };

            var result = await _userRepository.CreateAsync(user);

            return new UserResponseDto
            {
                Success = true,
                Message = "User created successfully",
                UserId = result._id,
                UserName = result.Name,
                PhoneNumber = result.PhoneNumber
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(UserRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.Identification);
            if (user == null)
            {
                return new UserResponseDto { Success = false, Message = "User not found" };
            }
            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Message = request.Message;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return new UserResponseDto { Success = true, Message = "User updated successfully" };
        }

        public async Task<UserResponseDto> DeleteUserAsync(string identification)
        {
            var user = await _userRepository.GetByIdAsync(identification);
            if (user == null)
            {
                return new UserResponseDto { Success = false, Message = "User not found" };
            }
            await _userRepository.DeleteByIdAsync(user._id);
            return new UserResponseDto { Success = true, Message = "User deleted successfully" };
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersListAsync();

            return users;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(string identification)
        {
            var user = await _userRepository.GetByIdAsync(identification);
            if (user == null)
            {
                return new UserResponseDto { Success = false, Message = "User not found" };
            }
            return new UserResponseDto { Success = true, Message = "User retrieved successfully", UserId = user._id, UserName = user.Name, PhoneNumber = user.PhoneNumber };
        }
    }
}