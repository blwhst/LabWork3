using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeStore.Services.Interfaces;
using ShoeStore.Services.Validators;
using ShoeStoreData.Contexts;
using ShoeStoreData.Models;
using ShoeStoreException;

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
        if (string.IsNullOrWhiteSpace(login))
            throw Exceptions.InvalidLogin(login ?? "");

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
            throw Exceptions.DuplicateLogin(user.Login);

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Создан новый пользователь: {Login} (ID: {Id})",
                user.Login, user.UserId);
            return user;
        }
        catch (Exception ex)
        {
            throw Exceptions.SaveChanges(ex);
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        UserValidator.Validate(user);

        var existing = await _context.Users.FindAsync(user.UserId);
        if (existing == null)
            throw Exceptions.UserNotFound(user.Login);

        existing.Login = user.Login;
        existing.LastName = user.LastName;
        existing.FirstName = user.FirstName;
        existing.MiddleName = user.MiddleName;
        existing.RoleId = user.RoleId;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Обновлён пользователь ID: {Id}", user.UserId);
        }
        catch (Exception ex)
        {
            throw Exceptions.SaveChanges(ex);
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Пользователь с Id {Id} не найден для удаления", id);
            throw Exceptions.UserNotFound($"ID {id}");
        }

        _context.Users.Remove(user);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Удалён пользователь ID: {Id}", id);
        }
        catch (Exception ex)
        {
            throw Exceptions.SaveChanges(ex);
        }
    }
}