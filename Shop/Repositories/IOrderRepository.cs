using Shop.Models;

namespace Shop.Repositories;

public interface IOrderRepository
{
    // Получить все заказы с фильтрацией
    Task<IEnumerable<Order>> GetAllAsync(
        int? clientId = null,
        string? status = null,
        decimal? amountFrom = null,
        decimal? amountTo = null);

    // Получить заказ по ID
    Task<Order?> GetByIdAsync(int id);

    // Добавить заказ
    Task<Order> AddAsync(Order order);

    // Обновить заказ
    Task<Order> UpdateAsync(Order order);

    // Удалить заказ
    Task<bool> DeleteAsync(int id);
}