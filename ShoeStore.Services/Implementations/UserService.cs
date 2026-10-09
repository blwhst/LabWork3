using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeStore.Services.Interfaces;
using ShoeStore.Services.Validators;
using ShoeStoreData.Contexts;
using ShoeStoreData.Models;

namespace ShoeStore.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        _logger.LogInformation("Поиск пользователя по логину: {Login}", login);
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .ToListAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        UserValidator.Validate(user);

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == user.Login);

        if (existingUser != null)
            throw new ArgumentException($"Пользователь с логином '{user.Login}' уже существует");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создан новый пользователь: {Login} (ID: {Id})", user.Login, user.UserId);
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        UserValidator.Validate(user);

        var existing = await _context.Users.FindAsync(user.UserId);
        if (existing == null)
            throw new Exception("Пользователь не найден");

        existing.Login = user.Login;
        existing.LastName = user.LastName;
        existing.FirstName = user.FirstName;
        existing.MiddleName = user.MiddleName;
        existing.RoleId = user.RoleId;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Обновлен пользователь ID: {Id}", user.UserId);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Пользователь с Id {Id} не найден для удаления", id);
            return;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Удален пользователь ID: {Id}", id);
    }
}