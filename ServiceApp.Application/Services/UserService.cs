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
            var existingUser = await _userRepository.GetByIdAsync(request.Identification);
            if (existingUser != null)
            {
                return new UserResponseDto { Success = false, Message = "La identificación ya está en uso" };
            }
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

            if (result == null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Message = "El telefono ya está en uso"
                };
            }
            
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

        public async Task<UserResponseDtoListPaginations> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            var users = await _userRepository.GetAllUsersListAsync(page, pageSize);
            var totalCount = await _userRepository.GetAllUsersCountAsync();
            var userResponseDataList = new List<UserResponseDtoList>();
            foreach (var user in users)
            {
                userResponseDataList.Add(new UserResponseDtoList { _id = user._id, Identification = user.Identification, Name = user.Name, PhoneNumber = user.PhoneNumber, Message = user.Message, MessageCount = user.MessageCount, Status = user.Status, CreatedAt = user.CreatedAt, UpdatedAt = user.UpdatedAt, LastMessageAt = user.LastMessageAt });
            }
            return new UserResponseDtoListPaginations { TotalCount = totalCount, Users = userResponseDataList, Page = page, PageSize = pageSize };
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
        public async Task<UserResponseDto> ResetCounterAsync()
        {
            await _userRepository.ResetAllCountersAsync();
            return new UserResponseDto { Success = true, Message = "Counter reset successfully" };
        }

    }
}