using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.Api.DTOs;
using ShoeStore.Api.Middleware;
using ShoeStore.Services.Interfaces;

namespace ShoeStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AuthController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userService.GetByLoginAsync(dto.Login);
        if (user == null)
            return Unauthorized(new { error = "Пользователь не найден" });

        var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();

        // Web будет хранить это у себя и передавать в заголовках при каждом запросе.
        return Ok(new LoginResponseDto
        {
            UserId = user.UserId,
            RoleId = user.RoleId,
            Login = user.Login,
            FullName = fullName
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout() => Ok();

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var login = HttpContext.GetLogin();
        if (string.IsNullOrEmpty(login)) return Unauthorized();

        var user = await _userService.GetByLoginAsync(login);
        if (user == null) return Unauthorized();

        return Ok(_mapper.Map<UserDto>(user));
    }
}