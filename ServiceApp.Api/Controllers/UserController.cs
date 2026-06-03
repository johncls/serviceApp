using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceApp.Application.DTOs;
using ServiceApp.Application.Services;

namespace ServiceApp.Api.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserRequestDto request)
        {
            var result = await _userService.CreateUserAsync(request);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser([FromBody] UserRequestDto request)
        {
            var result = await _userService.UpdateUserAsync(request);
            return Ok(result);
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("get-by-id")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(string identification)
        {
            var result = await _userService.GetUserByIdAsync(identification);
            return Ok(result);
        }
        
        [HttpDelete("delete")]
        public async Task<ActionResult<UserResponseDto>> DeleteUser(string identification)
        {
            var result = await _userService.DeleteUserAsync(identification);
            return Ok(result);
        }
    }
}