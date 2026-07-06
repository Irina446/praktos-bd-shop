using Shop.Models;

namespace Shop.Repositories;

public interface IClientRepository
{
    // Получить всех клиентов
    Task<IEnumerable<Client>> GetAllAsync();

    // Получить клиента по ID
    Task<Client?> GetByIdAsync(int id);

    // Добавить клиента
    Task<Client> AddAsync(Client client);

    // Обновить клиента
    Task<Client> UpdateAsync(Client client);

    // Удалить клиента
    Task<bool> DeleteAsync(int id);

    // Проверить, существует ли клиент
    Task<bool> ExistsAsync(int id);
}