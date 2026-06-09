using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceApp.Application.DTOs
{
    public class UserRequestDto
    {
        public string Identification { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UserLoginRequestDto
    {
        public string Identification { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}