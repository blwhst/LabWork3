using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.Api.DTOs;
using ShoeStore.Api.Middleware;
using ShoeStore.Services.Interfaces;
using ShoeStoreData.Models;

namespace ShoeStore.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public OrderController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }

    /// <summary>Список всех заказов. Менеджер и Админ.</summary>
    [HttpGet]
    [RoleAuthorize(Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    /// <summary>Мои заказы.</summary>
    [HttpGet("my")]
    [RoleAuthorize(Roles.User, Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<List<OrderDto>>> GetMy()
    {
        var userId = HttpContext.GetUserId();
        if (userId <= 0) return Unauthorized();

        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    /// <summary>Состав конкретного заказа.</summary>
    [HttpGet("{id:int}")]
    [RoleAuthorize(Roles.User, Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();

        // Обычный пользователь видит только свой заказ
        var roleId = HttpContext.GetRoleId();
        if (roleId.ToString() == Roles.User && order.UserId != HttpContext.GetUserId())
            return Forbid();

        return Ok(_mapper.Map<OrderDto>(order));
    }

    /// <summary>Создать заказ.</summary>
    [HttpPost]
    [RoleAuthorize(Roles.User, Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        var roleId = HttpContext.GetRoleId();
        var userId = HttpContext.GetUserId();

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ApiException("Заказ не может быть пустым", ApiStatus.BadRequest);

        // Клиент всегда создаётся от имени сессии, кроме Manager/Admin
        int effectiveUserId;
        if (roleId.ToString() == Roles.User)
        {
            effectiveUserId = userId;
        }
        else
        {
            if (dto.UserId <= 0)
                throw new ApiException("Не указан клиент", ApiStatus.BadRequest);
            effectiveUserId = dto.UserId;
        }

        // Схлопываем дубликаты ProductItemId (иначе получим нарушение UQ_OrderItem)
        var items = dto.Items
            .GroupBy(i => i.ProductItemId)
            .Select(g => new OrderItem
            {
                ProductItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        var order = new Order
        {
            UserId = effectiveUserId,
            OrderDate = DateOnly.FromDateTime(DateTime.Today),
            OrderItems = items
        };

        var created = await _orderService.CreateOrderAsync(order);
        var fresh = await _orderService.GetOrderByIdAsync(created.OrderId);
        return Ok(_mapper.Map<OrderDto>(fresh));
    }

    /// <summary>Редактировать. Только Админ.</summary>
    [HttpPut("{id:int}")]
    [RoleAuthorize(Roles.Admin)]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] UpdateOrderDto dto)
    {
        var existing = await _orderService.GetOrderByIdAsync(id);
        if (existing == null) return NotFound();

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ApiException("Заказ не может быть пустым", ApiStatus.BadRequest);

        // Схлопываем дубликаты — иначе сервис попытается вставить
        // две строки с одинаковым (OrderId, ProductItemId) → нарушение UQ_OrderItem
        var items = dto.Items
            .GroupBy(i => i.ProductItemId)
            .Select(g => new OrderItem
            {
                ProductItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        existing.UserId = dto.UserId;
        existing.OrderItems = items;

        await _orderService.UpdateOrderAsync(existing);

        // Перечитываем, чтобы вернуть актуальное состояние
        var updated = await _orderService.GetOrderByIdAsync(id);
        return Ok(_mapper.Map<OrderDto>(updated));
    }

    /// <summary>Удалить. Менеджер и Админ.</summary>
    [HttpDelete("{id:int}")]
    [RoleAuthorize(Roles.Manager, Roles.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _orderService.GetOrderByIdAsync(id);
        if (existing == null) return NotFound();

        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}