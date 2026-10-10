using ShoeStoreData.Models;
using ShoeStoreException;

namespace ShoeStore.Services.Validators;

public static class OrderValidator
{
    public static void Validate(Order order)
    {
        if (order == null)
            throw Exceptions.OrderIsNull();

        if (order.UserId <= 0)
            throw Exceptions.ClientRequired();

        if (order.OrderItems == null || !order.OrderItems.Any())
            throw Exceptions.EmptyOrder();

        foreach (var item in order.OrderItems)
        {
            if (item.Quantity <= 0)
                throw Exceptions.InvalidQuantity(item.Quantity);
        }
    }
}