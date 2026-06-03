namespace ServiceApp.Application.DTOs;

public class MessageResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
}
