using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoeStore.Api.DTOs;
using ShoeStore.Api.Middleware;
using ShoeStore.Services.Interfaces;
using ShoeStoreData.Models;
using ShoeStoreException;

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

    [HttpGet]
    [RoleAuthorize(Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    [HttpGet("my")]
    [RoleAuthorize(Roles.User, Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<List<OrderDto>>> GetMy()
    {
        var userId = HttpContext.GetUserId();
        if (userId <= 0) return Unauthorized();

        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    [HttpGet("{id:int}")]
    [RoleAuthorize(Roles.User, Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();

        var roleId = HttpContext.GetRoleId();
        if (roleId.ToString() == Roles.User && order.UserId != HttpContext.GetUserId())
            return StatusCode(403, new { error = "Недостаточно прав" });

        return Ok(_mapper.Map<OrderDto>(order));
    }

    [HttpPost]
    [RoleAuthorize(Roles.User, Roles.Manager, Roles.Admin)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        var roleId = HttpContext.GetRoleId();
        var userId = HttpContext.GetUserId();

        if (dto.Items == null || dto.Items.Count == 0)
            throw Exceptions.EmptyOrder();

        int effectiveUserId;
        if (roleId.ToString() == Roles.User)
        {
            effectiveUserId = userId;
        }
        else
        {
            if (dto.UserId <= 0)
                throw Exceptions.ClientRequired();
            effectiveUserId = dto.UserId;
        }

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

    [HttpPut("{id:int}")]
    [RoleAuthorize(Roles.Admin)]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] UpdateOrderDto dto)
    {
        var existing = await _orderService.GetOrderByIdAsync(id);
        if (existing == null) return NotFound();

        if (dto.Items == null || dto.Items.Count == 0)
            throw Exceptions.EmptyOrder();

        var items = dto.Items
            .GroupBy(i => i.ProductItemId)
            .Select(g => new OrderItem
            {
                ProductItemId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        var orderToUpdate = new Order
        {
            OrderId = id,
            UserId = dto.UserId,
            OrderDate = existing.OrderDate,
            OrderItems = items
        };

        await _orderService.UpdateOrderAsync(orderToUpdate);

        var updated = await _orderService.GetOrderByIdAsync(id);
        return Ok(_mapper.Map<OrderDto>(updated));
    }

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