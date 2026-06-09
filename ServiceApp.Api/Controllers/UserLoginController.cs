using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceApp.Application.DTOs;
using ServiceApp.Application.Services;

namespace ServiceApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLoginController
    {
        private readonly UserLoginService _userLoginService;

        public UserLoginController(UserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> LoginUser([FromBody] UserLoginRequestDto request)
        {
            var result = await _userLoginService.LoginUserAsync(request.Identification, request.Password);
            if (!result.Success)
            {
                 return new UserLoginResponseDto { Success = false, Message = "User not found" };
            }
            return result;
        }
    }
}