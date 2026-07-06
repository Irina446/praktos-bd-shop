using Microsoft.AspNetCore.Mvc;
using Shop.Models;
using Shop.Repositories;

namespace ShopAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _repository;

    // репозиторий
    public ClientsController(IClientRepository repository)
    {
        _repository = repository;
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var clients = await _repository.GetAllAsync();
        return Ok(clients);
    }

    // GET
    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client == null)
            return NotFound();
        return Ok(client);
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] Client client)
    {
        var created = await _repository.AddAsync(client);
        return CreatedAtAction(nameof(GetClient), new { id = created.ClientID }, created);
    }

    // PUT
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
    {
        if (id != client.ClientID)
            return BadRequest();

        if (!await _repository.ExistsAsync(id))
            return NotFound();

        await _repository.UpdateAsync(client);
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}