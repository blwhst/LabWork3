using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeStore.Services.Interfaces;
using ShoeStore.Services.Validators;
using ShoeStoreData.Contexts;
using ShoeStoreData.Models;

namespace ShoeStore.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(AppDbContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .ThenInclude(pi => pi.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .ThenInclude(pi => pi.Product)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .ThenInclude(pi => pi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        OrderValidator.Validate(order);

        foreach (var item in order.OrderItems)
        {
            var productItem = await _context.ProductItems.FindAsync(item.ProductItemId);

            if (productItem == null)
                throw new Exception($"Товар с ID {item.ProductItemId} не найден");

            if (productItem.Quantity < item.Quantity)
                throw new Exception($"Недостаточно товара '{productItem.ProductId}' на складе. Доступно: {productItem.Quantity}");

            productItem.Quantity -= item.Quantity;
        }

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Создан заказ ID: {OrderId} для пользователя {UserId}", order.OrderId, order.UserId);

        return order;
    }

    public async Task UpdateOrderAsync(Order order)
    {
        OrderValidator.Validate(order);

        var existingOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

        if (existingOrder == null) throw new Exception("Заказ не найден");

        foreach (var oldItem in existingOrder.OrderItems)
        {
            var productItem = await _context.ProductItems.FindAsync(oldItem.ProductItemId);
            if (productItem != null)
            {
                productItem.Quantity += oldItem.Quantity;
            }
        }

        _context.OrderItems.RemoveRange(existingOrder.OrderItems);

        foreach (var newItem in order.OrderItems)
        {
            var productItem = await _context.ProductItems.FindAsync(newItem.ProductItemId);

            if (productItem == null)
                throw new Exception($"Товар с ID {newItem.ProductItemId} не найден");

            if (productItem.Quantity < newItem.Quantity)
                throw new Exception($"Недостаточно товара на складе при обновлении. Доступно: {productItem.Quantity}");

            productItem.Quantity -= newItem.Quantity;
        }

        existingOrder.UserId = order.UserId;
        existingOrder.OrderDate = order.OrderDate;
        existingOrder.OrderItems = order.OrderItems;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Обновлен заказ ID: {OrderId}", order.OrderId);
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null) return;

        foreach (var item in order.OrderItems)
        {
            var productItem = await _context.ProductItems.FindAsync(item.ProductItemId);
            if (productItem != null)
            {
                productItem.Quantity += item.Quantity;
            }
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Удален заказ ID: {OrderId} и возвращен товар на склад", id);
    }
}