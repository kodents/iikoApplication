using iikoApplication.Data;
using iikoApplication.Interfaces;
using iikoApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace iikoApplication.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(AppDbContext context, ILogger<ClientRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Client?> GetByIdAsync(long clientId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Clients
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client by ID: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Clients
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all clients");
                throw;
            }
        }

        public async Task<bool> CreateAsync(Client client, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await _context.Clients
                    .AnyAsync(c => c.ClientId == client.ClientId, cancellationToken);

                if (exists)
                {
                    _logger.LogWarning("Duplicate client ID: {ClientId}", client.ClientId);
                    return false;
                }

                if (client.SystemId == Guid.Empty)
                {
                    client.SystemId = Guid.NewGuid();
                }

                await _context.Clients.AddAsync(client, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client: {ClientId}", client.ClientId);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Client client, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Clients.Update(client);
                var result = await _context.SaveChangesAsync(cancellationToken);
                return result > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client: {ClientId}", client.ClientId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long clientId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = new Client { ClientId = clientId };
                _context.Clients.Attach(client);
                _context.Clients.Remove(client);

                var result = await _context.SaveChangesAsync(cancellationToken);
                return result > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client: {ClientId}", clientId);
                throw;
            }
        }

        public async Task<IEnumerable<Client>> CreateMultipleAsync(IEnumerable<Client> clients, CancellationToken cancellationToken = default)
        {
            var failedClients = new List<Client>();

            var tasks = clients.Select(async client =>
            {
                try
                {
                    var exists = await _context.Clients
                        .AnyAsync(c => c.ClientId == client.ClientId, cancellationToken);

                    if (exists)
                    {
                        lock (failedClients) failedClients.Add(client);
                        return;
                    }

                    if (client.SystemId == Guid.Empty)
                        client.SystemId = Guid.NewGuid();

                    await _context.Clients.AddAsync(client, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing client: {ClientId}", client.ClientId);
                    lock (failedClients) failedClients.Add(client);
                }
            });

            await Task.WhenAll(tasks);
            await _context.SaveChangesAsync(cancellationToken);
            return failedClients;
        }
    }
}