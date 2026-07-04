using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(
        int? clientId = null,
        string? status = null,
        decimal? amountFrom = null,
        decimal? amountTo = null)
    {
        var query = _context.Orders
            .Include(o => o.Client)
            .AsQueryable();

        if (clientId.HasValue)
            query = query.Where(o => o.ClientID == clientId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        if (amountFrom.HasValue)
            query = query.Where(o => o.Amount >= amountFrom.Value);

        if (amountTo.HasValue)
            query = query.Where(o => o.Amount <= amountTo.Value);

        return await query.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Client)
            .FirstOrDefaultAsync(o => o.OrderID == id);
    }

    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await GetByIdAsync(id);
        if (order == null)
            return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}