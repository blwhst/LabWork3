using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeStore.Services.Interfaces;
using ShoeStore.Services.Validators;
using ShoeStoreData.Contexts;
using ShoeStoreData.Models;
using ShoeStoreException;

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

        if (order.OrderDate == default)
            order.OrderDate = DateOnly.FromDateTime(DateTime.Today);

        foreach (var item in order.OrderItems)
        {
            var productItem = await _context.ProductItems
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.ProductItemId == item.ProductItemId);

            if (productItem == null)
                throw Exceptions.ProductItemNotFound(item.ProductItemId);

            if (productItem.Quantity < item.Quantity)
            {
                var name = productItem.Product?.Name ?? $"ID {productItem.ProductId}";
                throw Exceptions.InsufficientStock(
                    name, productItem.Size, item.Quantity, productItem.Quantity);
            }

            item.UnitPrice = productItem.Product.Price;

            productItem.Quantity -= item.Quantity;
        }

        _context.Orders.Add(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw Exceptions.SaveChanges(ex);
        }

        _logger.LogInformation("Создан заказ ID: {OrderId} для пользователя {UserId}",
            order.OrderId, order.UserId);

        return order;
    }

    public async Task UpdateOrderAsync(Order order)
    {
        OrderValidator.Validate(order);

        var existingOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

        if (existingOrder == null)
            throw Exceptions.OrderNotFound(order.OrderId);

        var oldPrices = existingOrder.OrderItems
            .ToDictionary(i => i.ProductItemId, i => i.UnitPrice);

        foreach (var oldItem in existingOrder.OrderItems)
        {
            var productItem = await _context.ProductItems.FindAsync(oldItem.ProductItemId);
            if (productItem != null)
                productItem.Quantity += oldItem.Quantity;
        }

        _context.OrderItems.RemoveRange(existingOrder.OrderItems);

        foreach (var newItem in order.OrderItems)
        {
            var productItem = await _context.ProductItems
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.ProductItemId == newItem.ProductItemId);

            if (productItem == null)
                throw Exceptions.ProductItemNotFound(newItem.ProductItemId);

            if (productItem.Quantity < newItem.Quantity)
            {
                var name = productItem.Product?.Name ?? $"ID {productItem.ProductId}";
                throw Exceptions.InsufficientStock(
                    name, productItem.Size, newItem.Quantity, productItem.Quantity);
            }

            newItem.UnitPrice = oldPrices.TryGetValue(newItem.ProductItemId, out var oldPrice) 
                ? oldPrice
                : productItem.Product.Price;

            productItem.Quantity -= newItem.Quantity;
        }

        existingOrder.UserId = order.UserId;
        existingOrder.OrderDate = order.OrderDate;

        foreach (var newItem in order.OrderItems)
        {
            newItem.OrderId = existingOrder.OrderId;
            newItem.OrderItemId = 0;
            _context.OrderItems.Add(newItem);
        }

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Обновлён заказ ID: {OrderId}", order.OrderId);
        }
        catch (Exception ex)
        {
            throw Exceptions.OrderUpdate(order.OrderId, ex);
        }
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            throw Exceptions.OrderNotFound(id);

        foreach (var item in order.OrderItems)
        {
            var productItem = await _context.ProductItems.FindAsync(item.ProductItemId);
            if (productItem != null)
                productItem.Quantity += item.Quantity;
        }

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Удалён заказ ID: {OrderId}, товар возвращён на склад", id);
        }
        catch (Exception ex)
        {
            throw Exceptions.OrderDelete(id, ex);
        }
    }
}