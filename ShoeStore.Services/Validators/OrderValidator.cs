using ShoeStoreData.Models;

namespace ShoeStore.Services.Validators;

public static class OrderValidator
{
    public static void Validate(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order), "Заказ не может быть пустым");

        if (order.UserId <= 0)
            throw new ArgumentException("Не указан клиент");

        if (order.OrderItems == null || !order.OrderItems.Any())
            throw new ArgumentException("Заказ не может быть пустым");

        foreach (var item in order.OrderItems)
        {
            if (item.Quantity <= 0)
                throw new ArgumentException($"Количество для позиции {item.ProductItemId} должно быть больше 0");
        }
    }
}