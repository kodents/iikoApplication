using iikoApplication.Models;

namespace iikoApplication.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(long clientId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(Client client, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Client client, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(long clientId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Client>> CreateMultipleAsync(IEnumerable<Client> clients, CancellationToken cancellationToken = default);
    }
}