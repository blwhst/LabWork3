namespace ShoeStoreException;

public static class Exceptions
{
    public static BaseException UserNotFound(string login) =>
        new($"Пользователь с логином {login} не найден");

    public static BaseException InvalidLogin(string login) =>
        new($"Некорректный логин: {login}");

    public static BaseException DuplicateLogin(string login) =>
        new($"Логин {login} уже используется");

    public static BaseException RoleNotFound(string roleName) =>
        new($"Роль {roleName} не найдена");

    public static BaseException Unauthorized() =>
        new("Требуется авторизация для выполнения действия");

    public static BaseException AccessDenied(string action) =>
        new($"Доступ к действию {action} запрещён");

    public static BaseException ProductNotFound(string productName, string manufacturer) =>
        new($"Товар {productName} ({manufacturer}) не найден");

    public static BaseException CategoryNotFound(string categoryName) =>
        new($"Категория {categoryName} не найдена");

    public static BaseException SubcategoryNotFound(string subcategoryName) =>
        new($"Подкатегория {subcategoryName} не найдена");

    public static BaseException ImageNotFound(string productName) =>
        new($"У товара {productName} отсутствует изображение");

    public static BaseException ProductItemNotFound(string productName, decimal size) =>
        new($"Товарная позиция {productName} размера {size} не найдена");

    public static BaseException InsufficientStock(string productName, decimal size, int requested, int available) =>
        new(available <= 0
            ? $"Товара {productName} размера {size} нет в наличии"
            : $"Товара {productName} размера {size} запрошено {requested}, в наличии {available}");

    public static BaseException OrderNotFound(int orderId) =>
        new($"Заказ №{orderId} не найден");

    public static BaseException OrderItemNotFound(int orderId, string productName, decimal size) =>
        new($"В заказе №{orderId} нет позиции {productName} размера {size}");

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

    public static BaseException DatabaseConnection(Exception innerException) =>
        new("Нет связи с базой данных", innerException);

    public static BaseException SaveChanges(Exception innerException) =>
        new("Ошибка при сохранении данных", innerException);

    public static BaseException Unknown(Exception innerException) =>
        new($"Непредвиденное исключение: {innerException.Message}", innerException);
}