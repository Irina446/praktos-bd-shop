using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.Repositories;

namespace ShopAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IClientRepository _clientRepository;

    public OrdersController(IOrderRepository orderRepository, IClientRepository clientRepository)
    {
        _orderRepository = orderRepository;
        _clientRepository = clientRepository;
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int? clientId,
        [FromQuery] string? status,
        [FromQuery] decimal? amountFrom,
        [FromQuery] decimal? amountTo)
    {
        var orders = await _orderRepository.GetAllAsync(clientId, status, amountFrom, amountTo);
        return Ok(orders);
    }

    // GET
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        if (!await _clientRepository.ExistsAsync(order.ClientID))
            return BadRequest("Клиент не найден");

        var created = await _orderRepository.AddAsync(order);
        return CreatedAtAction(nameof(GetOrder), new { id = created.OrderID }, created);
    }

    // PUT
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
    {
        if (id != order.OrderID)
            return BadRequest();

        var existing = await _orderRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _orderRepository.UpdateAsync(order);
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var deleted = await _orderRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}