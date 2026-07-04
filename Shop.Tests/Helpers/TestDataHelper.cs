using Shop.Models;

namespace Shop.Tests.Helpers;

public static class TestDataHelper
{
    public static List<Client> GetTestClients()
    {
        return new List<Client>
        {
            new Client
            {
                ClientID = 1,
                FirstName = "Иван",
                LastName = "Петров",
                BirthDate = new DateTime(1990, 5, 15)
            },
            new Client
            {
                ClientID = 2,
                FirstName = "Екатерина",
                LastName = "Иванова",
                BirthDate = new DateTime(1985, 12, 10)
            },
            new Client
            {
                ClientID = 3,
                FirstName = "Никита",
                LastName = "Сидоров",
                BirthDate = new DateTime(2000, 3, 25)
            }
        };
    }

    public static List<Order> GetTestOrders()
    {
        return new List<Order>
        {
            new Order
            {
                OrderID = 1,
                ClientID = 1,
                Amount = 1500.50m,
                OrderDateTime = new DateTime(2026, 5, 15, 10, 30, 0),
                Status = "Выполнен"
            },
            new Order
            {
                OrderID = 2,
                ClientID = 1,
                Amount = 2300.00m,
                OrderDateTime = new DateTime(2026, 5, 15, 14, 15, 0),
                Status = "Выполнен"
            },
            new Order
            {
                OrderID = 3,
                ClientID = 2,
                Amount = 3200.00m,
                OrderDateTime = new DateTime(2026, 6, 1, 16, 30, 0),
                Status = "Отменен"
            }
        };
    }
}