using ShoeStoreData.Models;
using ShoeStoreException;

namespace ShoeStore.Services.Validators;

public static class UserValidator
{
    public static void Validate(User user)
    {
        if (user == null)
            throw Exceptions.UserIsNull();

        if (string.IsNullOrWhiteSpace(user.Login))
            throw Exceptions.InvalidLogin(user.Login ?? "");

        if (user.Login.Length > 30)
            throw Exceptions.LoginTooLong();

        if (string.IsNullOrWhiteSpace(user.LastName))
            throw Exceptions.LastNameRequired();

        if (user.LastName.Length > 50)
            throw Exceptions.LastNameTooLong();

        if (string.IsNullOrWhiteSpace(user.FirstName))
            throw Exceptions.FirstNameRequired();

        if (user.FirstName.Length > 50)
            throw Exceptions.FirstNameTooLong();

        if (!string.IsNullOrWhiteSpace(user.MiddleName) && user.MiddleName.Length > 50)
            throw Exceptions.MiddleNameTooLong();

        if (user.RoleId <= 0)
            throw Exceptions.RoleRequired();
    }
}