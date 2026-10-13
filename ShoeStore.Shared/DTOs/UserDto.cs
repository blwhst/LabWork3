namespace ShoeStore.Api.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
