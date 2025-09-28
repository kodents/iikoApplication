using iikoApplication.Interfaces;
using iikoApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace iikoApplication.Controllers
{
    /// <summary>API for managing clients</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientRepository clientRepository, ILogger<ClientsController> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        /// <summary>Get all clients</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Client>), 200)]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients(CancellationToken cancellationToken = default)
        {
            try
            {
                var clients = await _clientRepository.GetAllAsync(cancellationToken);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>Get client by ID</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Client>> GetClient(long id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = await _clientRepository.GetByIdAsync(id, cancellationToken);
                return client == null ? NotFound() : Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client: {ClientId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>Create new client</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Client), 201)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<Client>> CreateClient(
            [FromBody] ClientDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = new Client
                {
                    ClientId = request.ClientId,
                    Username = request.Username
                };

                var success = await _clientRepository.CreateAsync(client, cancellationToken);
                if (!success) return Conflict("Client with this ID already exists");

                return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client: {ClientId}", request.ClientId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>Update client</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClient(
            long id,
            [FromBody] ClientDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id != request.ClientId) return BadRequest("Route ID doesn't match request ClientId");

                var existingClient = await _clientRepository.GetByIdAsync(id, cancellationToken);
                if (existingClient == null) return NotFound();

                existingClient.Username = request.Username;
                var success = await _clientRepository.UpdateAsync(existingClient, cancellationToken);

                return success ? NoContent() : StatusCode(500, "Update failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client: {ClientId}", request.ClientId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>Delete client</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteClient(long id, CancellationToken cancellationToken = default)
        {
            try
            {
                var success = await _clientRepository.DeleteAsync(id, cancellationToken);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client: {ClientId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>Create multiple clients (min 10)</summary>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(IEnumerable<Client>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Client>>> CreateMultipleClients(
            [FromBody] ClientDto[] clients,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (clients.Length < 10) return BadRequest("At least 10 clients required");

                var domainClients = clients.Select(request => new Client
                {
                    ClientId = request.ClientId,
                    Username = request.Username
                });

                var failedClients = await _clientRepository.CreateMultipleAsync(domainClients, cancellationToken);
                return Ok(failedClients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch operation");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}