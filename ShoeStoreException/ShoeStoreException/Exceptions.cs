namespace ShoeStoreException;

public static class Exceptions
{
    public static BaseException UserNotFound(string login) =>
        new($"Пользователь с логином {login} не найден");

    public static BaseException UserNotFound(int userId) =>
    new($"Пользователь с идентификатором {userId} не найден");

    public static BaseException InvalidLogin(string login) =>
        new($"Некорректный логин: {login}");

    public static BaseException DuplicateLogin(string login) =>
        new($"Логин {login} уже используется");

    public static BaseException ProductNotFound(string productName, string manufacturer) =>
        new($"Товар {productName} ({manufacturer}) не найден");

    public static BaseException ProductNotFound(int productId) =>
        new($"Товар с идентификатором {productId} не найден");

    public static BaseException CategoryNotFound(int categoryId) =>
    new($"Категория с идентификатором {categoryId} не найдена");

    public static BaseException ProductItemNotFound(int productItemId) =>
        new($"Товарная позиция с идентификатором {productItemId} не найдена");

    public static BaseException InsufficientStock(string productName, decimal size, int requested, int available) =>
        new(available <= 0
            ? $"Товара {productName} размера {size} нет в наличии"
            : $"Товара {productName} размера {size} запрошено {requested}, в наличии {available}");

    public static BaseException OrderNotFound(int orderId) =>
        new($"Заказ №{orderId} не найден");

    public static BaseException EmptyOrder() =>
        new("Заказ не содержит ни одной товарной позиции");

    public static BaseException OrderUpdate(int orderId, Exception innerException) =>
        new($"Не удалось изменить заказ №{orderId}", innerException);

    public static BaseException OrderDelete(int orderId, Exception innerException) =>
        new($"Не удалось удалить заказ №{orderId}", innerException);

    public static BaseException InvalidQuantity(int quantity) =>
        new($"Некорректное количество: {quantity}. Должно быть больше нуля");

    public static BaseException InvalidPrice(decimal price) =>
        new($"Некорректная цена: {price}. Должна быть больше нуля");

    public static BaseException ProductIsNull() =>
        new("Не выбран товар");

    public static BaseException ProductNameRequired() =>
        new("Название товара обязательно");

    public static BaseException ManufacturerRequired() =>
        new("Производитель обязателен");

    public static BaseException SubcategoryRequired() =>
        new("Необходимо указать подкатегорию");

    public static BaseException UserIsNull() =>
        new("Пользователь не указан");

    public static BaseException LastNameRequired() =>
        new("Фамилия обязательна для заполнения");

    public static BaseException FirstNameRequired() =>
        new("Имя обязательно для заполнения");

    public static BaseException LastNameTooLong() =>
        new("Фамилия не может быть длиннее 50 символов");

    public static BaseException FirstNameTooLong() =>
        new("Имя не может быть длиннее 50 символов");

    public static BaseException MiddleNameTooLong() =>
        new("Отчество не может быть длиннее 50 символов");

    public static BaseException LoginTooLong() =>
        new("Логин не может быть длиннее 30 символов");

    public static BaseException RoleRequired() =>
        new("Необходимо указать роль пользователя");

    public static BaseException OrderIsNull() =>
        new("Заказ не указан");

    public static BaseException ClientRequired() =>
        new("Не указан клиент");

    public static BaseException DatabaseConnection(Exception innerException) =>
        new("Связь с базой данных потеряна", innerException);

    public static BaseException SaveChanges(Exception innerException) =>
        new("Ошибка при сохранении данных", innerException);

    public static BaseException Unknown(Exception innerException) =>
        new($"Непредвиденное исключение: {innerException.Message}", innerException);
}