using ShoeStoreData.Models;

namespace ShoeStore.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByLoginAsync(string login);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
}