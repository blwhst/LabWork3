using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.Api.DTOs;
using ShoeStore.Api.Middleware;
using ShoeStore.Services.Interfaces;

namespace ShoeStore.Api.Controllers;

[ApiController]
[Route("api/users")]
[RoleAuthorize(Roles.Admin)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(_mapper.Map<List<UserDto>>(users));
    }
}