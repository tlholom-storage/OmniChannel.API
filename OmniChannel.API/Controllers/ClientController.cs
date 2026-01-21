using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OmniChannel.API.DTOs;
using OmniChannel.API.Hubs;
using OmniChannel.API.Mappings;
using OmniChannel.API.Repositories;
using OmniChannel.API.Services;

namespace OmniChannel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _repo;
        private readonly ILogService _log;

        public ClientController(
            IClientRepository repo,
            ILogService log)
        {
            _repo = repo;
            _log = log;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            await _log.LogAsync("Fetching all clients");

            var clients = await _repo.GetAllAsync();

            await _log.LogAsync($"Returned {clients.Count()} clients");

            return Ok(clients.Select(c => c.ToDto()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            await _log.LogAsync($"Fetching client with ID {id}");

            var client = await _repo.GetByIdAsync(id);

            if (client == null)
            {
                await _log.LogAsync($"Client with ID {id} not found", "WARN");
                return NotFound();
            }

            await _log.LogAsync($"Client with ID {id} retrieved successfully");

            return Ok(client.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto dto)
        {
            if (!ModelState.IsValid)
            {
                await _log.LogAsync("Invalid client creation request", "WARN");
                return BadRequest(ModelState);
            }

            await _log.LogAsync($"Creating new client: {dto.FullName}");

            var client = dto.ToEntity();
            var created = await _repo.CreateAsync(client);

            await _log.LogAsync($"Client created with ID {created.ClientID}", "INFO");

            return CreatedAtAction(
                nameof(GetClient),
                new { id = created.ClientID },
                created.ToDto()
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientDto dto)
        {
            if (!ModelState.IsValid)
            {
                await _log.LogAsync($"Invalid update request for client {id}", "WARN");
                return BadRequest(ModelState);
            }

            if (!await _repo.ExistsAsync(id))
            {
                await _log.LogAsync($"Update failed — client {id} does not exist", "WARN");
                return NotFound();
            }

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
            {
                await _log.LogAsync($"Client {id} disappeared during update", "ERROR");
                return NotFound();
            }

            existing.ApplyUpdates(dto);
            await _repo.UpdateAsync(existing);

            await _log.LogAsync($"Client {id} updated successfully");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            if (!await _repo.ExistsAsync(id))
            {
                await _log.LogAsync($"Delete failed — client {id} not found", "WARN");
                return NotFound();
            }

            await _repo.DeleteAsync(id);

            await _log.LogAsync($"Client {id} deleted", "WARN");

            return NoContent();
        }
    }
}
