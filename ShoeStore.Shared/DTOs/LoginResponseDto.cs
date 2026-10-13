namespace ShoeStore.Api.DTOs;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}