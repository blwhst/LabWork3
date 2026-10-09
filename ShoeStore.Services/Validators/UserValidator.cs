using ShoeStoreData.Models;

namespace ShoeStore.Services.Validators
{
    public static class UserValidator
    {
        public static void Validate(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "Пользователь не может быть пустым");

            if (string.IsNullOrWhiteSpace(user.Login))
                throw new ArgumentException("Логин обязателен для заполнения");

            if (user.Login.Length > 30)
                throw new ArgumentException("Логин не может быть длиннее 30 символов");

            if (string.IsNullOrWhiteSpace(user.LastName))
                throw new ArgumentException("Фамилия обязательна для заполнения");

            if (user.LastName.Length > 50)
                throw new ArgumentException("Фамилия не может быть длиннее 50 символов");

            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new ArgumentException("Имя обязательно для заполнения");

            if (user.FirstName.Length > 50)
                throw new ArgumentException("Имя не может быть длиннее 50 символов");

            if (!string.IsNullOrWhiteSpace(user.MiddleName) && user.MiddleName.Length > 50)
                throw new ArgumentException("Отчество не может быть длиннее 50 символов");

            if (user.RoleId <= 0)
                throw new ArgumentException("Необходимо указать роль пользователя");
        }
    }
}