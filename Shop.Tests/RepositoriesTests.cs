using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shop.Data;
using Shop.Models;
using Shop.Repositories;
using ShopAPI.Controllers;
using Xunit;

namespace Shop.Tests.Repositories;

public class RepositoriesTests
{

    /// <summary>
    /// Создаёт временную базу данных в памяти для тестов
    /// </summary>
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        // тестовые данные
        context.Clients.AddRange(new List<Client>
        {
            new Client { ClientID = 1, FirstName = "Иван", LastName = "Петров", BirthDate = new DateTime(1990, 5, 15) },
            new Client { ClientID = 2, FirstName = "Екатерина", LastName = "Иванова", BirthDate = new DateTime(1985, 12, 10) },
            new Client { ClientID = 3, FirstName = "Никита", LastName = "Сидоров", BirthDate = new DateTime(2000, 3, 25) }
        });

        context.Orders.AddRange(new List<Order>
        {
            new Order { OrderID = 1, ClientID = 1, Amount = 1500.50m, OrderDateTime = new DateTime(2026, 5, 15, 10, 30, 0), Status = "Выполнен" },
            new Order { OrderID = 2, ClientID = 1, Amount = 2300.00m, OrderDateTime = new DateTime(2026, 5, 15, 14, 15, 0), Status = "Выполнен" },
            new Order { OrderID = 3, ClientID = 2, Amount = 3200.00m, OrderDateTime = new DateTime(2026, 6, 1, 16, 30, 0), Status = "Отменен" }
        });

        context.SaveChanges();
        return context;
    }

    // тест репозитория клиентов

    /// <summary>
    /// тест 1. проверка, что метод возвращает всех клиентов из БД
    /// </summary>
    [Fact]
    public async Task ClientRepository_GetAllAsync_ShouldReturnAllClients()
    {
        using var context = GetInMemoryDbContext();
        var repository = new ClientRepository(context);

        var result = await repository.GetAllAsync();

        Assert.Equal(3, result.Count());
    }

    /// <summary>
    /// тест 2. проверка получение клиента по существующему ID
    /// </summary>
    [Fact]
    public async Task ClientRepository_GetByIdAsync_WithValidId_ShouldReturnClient()
    {
        using var context = GetInMemoryDbContext();
        var repository = new ClientRepository(context);

        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.ClientID);
        Assert.Equal("Иван", result.FirstName);
    }

    /// <summary>
    /// тест 3. Проверка, что при запросе несуществующего ID возвращается null
    /// </summary>
    [Fact]
    public async Task ClientRepository_GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        using var context = GetInMemoryDbContext();
        var repository = new ClientRepository(context);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    /// <summary>
    /// тест 4. Проверяет добавление нового клиента в БД
    /// </summary>
    [Fact]
    public async Task ClientRepository_AddAsync_ShouldAddClient()
    {
        using var context = GetInMemoryDbContext();
        var repository = new ClientRepository(context);
        var newClient = new Client
        {
            FirstName = "Ольга",
            LastName = "Козлова",
            BirthDate = new DateTime(1995, 9, 3)
        };

        var result = await repository.AddAsync(newClient);

        Assert.NotNull(result);
        Assert.Equal(4, result.ClientID);
        Assert.Equal(4, context.Clients.Count());
    }

    // тесты репозитория заказов 

    /// <summary>
    /// Тест 5. Проверяет, что метод возвращает все заказы
    /// </summary>
    [Fact]
    public async Task OrderRepository_GetAllAsync_ShouldReturnAllOrders()
    {
        using var context = GetInMemoryDbContext();
        var repository = new OrderRepository(context);

        var result = await repository.GetAllAsync();

        Assert.Equal(3, result.Count());
    }

    /// <summary>
    /// Тест 6. Проверяет фильтрацию заказов по ID клиента
    /// </summary>
    [Fact]
    public async Task OrderRepository_GetAllAsync_WithClientIdFilter_ShouldReturnFilteredOrders()
    {
        using var context = GetInMemoryDbContext();
        var repository = new OrderRepository(context);

        var result = await repository.GetAllAsync(clientId: 1);

        Assert.Equal(2, result.Count());
        Assert.All(result, o => Assert.Equal(1, o.ClientID));
    }

    /// <summary>
    /// тест 7. Проверяет получение заказа по существующему ID
    /// </summary>
    [Fact]
    public async Task OrderRepository_GetByIdAsync_WithValidId_ShouldReturnOrder()
    {
        using var context = GetInMemoryDbContext();
        var repository = new OrderRepository(context);

        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.OrderID);
        Assert.Equal(1500.50m, result.Amount);
    }

    /// <summary>
    /// тест 8. Проверяет добавление нового заказа в БД
    /// </summary>
    [Fact]
    public async Task OrderRepository_AddAsync_ShouldAddOrder()
    {
        using var context = GetInMemoryDbContext();
        var repository = new OrderRepository(context);
        var newOrder = new Order
        {
            ClientID = 1,
            Amount = 500.00m,
            OrderDateTime = DateTime.Now,
            Status = "Не обработан"
        };

        var result = await repository.AddAsync(newOrder);

        Assert.NotNull(result);
        Assert.Equal(4, result.OrderID);
        Assert.Equal(4, context.Orders.Count());
    }

    // тесты контроллера клиентов

    /// <summary>
    /// Тест 9. Проверяет, что возвращаются список все клиенты
    /// </summary>
    [Fact]
    public async Task ClientsController_GetClients_ShouldReturnAllClients()
    {
        var mockRepo = new Mock<IClientRepository>();
        var clients = new List<Client>
        {
            new Client { ClientID = 1, FirstName = "Иван", LastName = "Петров" },
            new Client { ClientID = 2, FirstName = "Екатерина", LastName = "Иванова" }
        };
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(clients);

        var controller = new ClientsController(mockRepo.Object);
        var result = await controller.GetClients();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedClients = Assert.IsType<List<Client>>(okResult.Value);
        Assert.Equal(2, returnedClients.Count);
    }

    /// <summary>
    /// Тест 10. Проверяет, что /api/clients/1 возвращает клиента с ID=1
    /// </summary>
    [Fact]
    public async Task ClientsController_GetClient_WithValidId_ShouldReturnClient()
    {
        var mockRepo = new Mock<IClientRepository>();
        var client = new Client { ClientID = 1, FirstName = "Иван", LastName = "Петров" };
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(client);

        var controller = new ClientsController(mockRepo.Object);
        var result = await controller.GetClient(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedClient = Assert.IsType<Client>(okResult.Value);
        Assert.Equal(1, returnedClient.ClientID);
    }

    /// <summary>
    /// Тест 11. Проверяет, что при запросе несуществующего клиента возвращается 404
    /// </summary>
    [Fact]
    public async Task ClientsController_GetClient_WithInvalidId_ShouldReturnNotFound()
    {
        var mockRepo = new Mock<IClientRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Client?)null);

        var controller = new ClientsController(mockRepo.Object);
        var result = await controller.GetClient(999);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Тест 12. Проверяет создание нового клиента через API
    /// </summary>
    [Fact]
    public async Task ClientsController_CreateClient_ShouldAddAndReturnClient()
    {
        var mockRepo = new Mock<IClientRepository>();
        var newClient = new Client { FirstName = "Ольга", LastName = "Козлова" };
        var createdClient = new Client { ClientID = 5, FirstName = "Ольга", LastName = "Козлова" };
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Client>())).ReturnsAsync(createdClient);

        var controller = new ClientsController(mockRepo.Object);
        var result = await controller.CreateClient(newClient);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedClient = Assert.IsType<Client>(createdResult.Value);
        Assert.Equal(5, returnedClient.ClientID);
    }

    // тесты контроллера заказов

    /// <summary>
    /// Тест 13. Проверяет, что /api/orders возвращает список всех заказов
    /// </summary>
    [Fact]
    public async Task OrdersController_GetOrders_ShouldReturnAllOrders()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockClientRepo = new Mock<IClientRepository>();

        var orders = new List<Order>
        {
            new Order { OrderID = 1, ClientID = 1, Amount = 1500.50m, Status = "Выполнен" },
            new Order { OrderID = 2, ClientID = 1, Amount = 2300.00m, Status = "Выполнен" }
        };
        mockOrderRepo.Setup(r => r.GetAllAsync(null, null, null, null)).ReturnsAsync(orders);

        var controller = new OrdersController(mockOrderRepo.Object, mockClientRepo.Object);
        var result = await controller.GetOrders(null, null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOrders = Assert.IsType<List<Order>>(okResult.Value);
        Assert.Equal(2, returnedOrders.Count);
    }

    /// <summary>
    /// Тест 14. Проверяет получение заказа по существующему ID через API
    /// </summary>
    [Fact]
    public async Task OrdersController_GetOrder_WithValidId_ShouldReturnOrder()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockClientRepo = new Mock<IClientRepository>();

        var order = new Order { OrderID = 1, ClientID = 1, Amount = 1500.50m, Status = "Выполнен" };
        mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        var controller = new OrdersController(mockOrderRepo.Object, mockClientRepo.Object);
        var result = await controller.GetOrder(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOrder = Assert.IsType<Order>(okResult.Value);
        Assert.Equal(1, returnedOrder.OrderID);
    }

    /// <summary>
    /// Тест 15. Проверяет создание заказа через API при существующем клиенте
    /// </summary>
    [Fact]
    public async Task OrdersController_CreateOrder_WithValidClient_ShouldCreateOrder()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockClientRepo = new Mock<IClientRepository>();

        var newOrder = new Order { ClientID = 1, Amount = 500.00m, Status = "Не обработан" };
        var createdOrder = new Order { OrderID = 10, ClientID = 1, Amount = 500.00m, Status = "Не обработан" };

        mockClientRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        mockOrderRepo.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder);

        var controller = new OrdersController(mockOrderRepo.Object, mockClientRepo.Object);
        var result = await controller.CreateOrder(newOrder);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedOrder = Assert.IsType<Order>(createdResult.Value);
        Assert.Equal(10, returnedOrder.OrderID);
    }

    /// <summary>
    /// Тест 16. Проверяет, что при создании заказа с несуществующим клиентом возвращается ошибка
    /// </summary>
    [Fact]
    public async Task OrdersController_CreateOrder_WithInvalidClient_ShouldReturnBadRequest()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockClientRepo = new Mock<IClientRepository>();

        var newOrder = new Order { ClientID = 999, Amount = 500.00m, Status = "Не обработан" };
        mockClientRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        var controller = new OrdersController(mockOrderRepo.Object, mockClientRepo.Object);
        var result = await controller.CreateOrder(newOrder);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Клиент не найден", badRequestResult.Value);
    }
}