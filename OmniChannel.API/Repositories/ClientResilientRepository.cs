using OmniChannel.API.Models;
using OmniChannel.API.Services;
using Polly;
using Polly.Retry;

namespace OmniChannel.API.Repositories
{
    public class ClientResilientRepository : IClientRepository
    {
        private readonly ClientSQLRepository _sqlRepo;
        private readonly ClientAzureTableRepository _noSqlRepo;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogService _logger;

        public ClientResilientRepository(
            ClientSQLRepository sqlRepo,
            ClientAzureTableRepository noSqlRepo,
            ILogService logger)
        {
            _sqlRepo = sqlRepo;
            _noSqlRepo = noSqlRepo;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 1,
                    sleepDurationProvider: attempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, attempt))
                );
        }

        private Task LogFailoverAsync(string operation, Exception ex)
        {
            return _logger.LogAsync(
                $"FAILOVER triggered | Operation={operation} | Reason={ex.Message}",
                "CRITICAL"
            );
        }

        public async Task<Client> CreateAsync(Client client)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(
                    () => _sqlRepo.CreateAsync(client));
            }
            catch (Exception ex)
            {
                await LogFailoverAsync("CreateClient", ex);
                return await _noSqlRepo.CreateAsync(client);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(
                    () => _sqlRepo.DeleteAsync(id));
            }
            catch (Exception ex)
            {
                await LogFailoverAsync($"DeleteClient Id={id}", ex);
                await _noSqlRepo.DeleteAsync(id);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _sqlRepo.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                await LogFailoverAsync($"ExistsClient Id={id}", ex);
                return await _noSqlRepo.ExistsAsync(id);
            }
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            try
            {
                return await _sqlRepo.GetAllAsync();
            }
            catch (Exception ex)
            {
                await LogFailoverAsync("GetAllClients", ex);
                return await _noSqlRepo.GetAllAsync();
            }
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            try
            {
                return await _sqlRepo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                await LogFailoverAsync($"GetClient Id={id}", ex);
                return await _noSqlRepo.GetByIdAsync(id);
            }
        }

        public async Task UpdateAsync(Client client)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(
                    () => _sqlRepo.UpdateAsync(client));
            }
            catch (Exception ex)
            {
                await LogFailoverAsync($"UpdateClient Id={client.ClientID}", ex);
                await _noSqlRepo.UpdateAsync(client);
            }
        }
    }
}
