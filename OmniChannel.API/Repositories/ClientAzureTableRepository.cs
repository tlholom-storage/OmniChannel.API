using Azure;
using Azure.Data.Tables;
using OmniChannel.API.Models;

namespace OmniChannel.API.Repositories
{
    public class ClientAzureTableRepository : IClientRepository
    {
        private readonly TableClient _tableClient;

        private const string ClientPartition = "Clients";
        private const string CounterPartition = "Counters";
        private const string CounterRow = "ClientId";

        public ClientAzureTableRepository(TableServiceClient serviceClient)
        {
            _tableClient = serviceClient.GetTableClient("Clients");
            _tableClient.CreateIfNotExists();
        }

        public async Task<Client> CreateAsync(Client client)
        {
            var generatedClientId = await GetNextClientIdAsync();

            var entity = new TableEntity(ClientPartition, client.Email.Replace(".", "_"))
            {
                { "ClientID", generatedClientId },
                { "FullName", client.FullName },
                { "Email", client.Email },
                { "Status", client.Status },
                { "AssignedManagerEmail", client.AssignedManagerEmail },
                { "LastModifiedBy", "Failover-NoSQL" },
                { "CreatedAt", DateTime.UtcNow }
            };

            await _tableClient.AddEntityAsync(entity);

            client.ClientID = generatedClientId;
            return client;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetEntityByIdInternal(id);
            if (entity != null)
            {
                await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return (await GetEntityByIdInternal(id)) != null;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            var clients = new List<Client>();
            var query = _tableClient.QueryAsync<TableEntity>(e => e.PartitionKey == ClientPartition);

            await foreach (var entity in query)
            {
                clients.Add(MapToClient(entity));
            }

            return clients;
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            var entity = await GetEntityByIdInternal(id);
            return entity != null ? MapToClient(entity) : null;
        }

        public async Task UpdateAsync(Client client)
        {
            var entity = new TableEntity(ClientPartition, client.Email.Replace(".", "_"))
            {
                { "ClientID", client.ClientID },
                { "FullName", client.FullName },
                { "Status", client.Status },
                { "LastModifiedBy", "Failover-Update" }
            };

            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Merge);
        }

        private async Task<TableEntity?> GetEntityByIdInternal(int id)
        {
            var query = _tableClient.QueryAsync<TableEntity>(
                e => e.PartitionKey == ClientPartition && e.GetInt32("ClientID") == id
            );

            await foreach (var entity in query)
            {
                return entity;
            }

            return null;
        }

        private async Task<int> GetNextClientIdAsync()
        {
            try
            {
                var counter = await _tableClient.GetEntityAsync<TableEntity>(
                    CounterPartition,
                    CounterRow
                );

                var current = counter.Value.GetInt32("LastClientId") ?? 0;
                var next = current + 1;

                counter.Value["LastClientId"] = next;
                await _tableClient.UpdateEntityAsync(counter.Value, counter.Value.ETag);

                return next;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                var counter = new TableEntity(CounterPartition, CounterRow)
                {
                    { "LastClientId", 1 }
                };

                await _tableClient.AddEntityAsync(counter);
                return 1;
            }
        }

        private Client MapToClient(TableEntity entity) => new Client
        {
            ClientID = entity.GetInt32("ClientID") ?? 0,
            FullName = entity.GetString("FullName") ?? "",
            Email = entity.GetString("Email") ?? "",
            Status = entity.GetString("Status") ?? "Active",
            CreatedAt = entity.GetDateTimeOffset("CreatedAt")?.UtcDateTime ?? DateTime.UtcNow
        };
    }
}
