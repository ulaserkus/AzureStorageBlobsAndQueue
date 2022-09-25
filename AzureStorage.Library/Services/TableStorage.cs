using Azure.Data.Tables;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureStorage.Library.Services
{
    public class TableStorage<TEntity> : INoSqlStorage<TEntity> where TEntity : class, ITableEntity, new()
    {

        private readonly TableClient _cloudTable;
        private readonly TableServiceClient _cloudTableClient;

        public TableStorage()       
        {
            _cloudTableClient = new TableServiceClient(ConnectionStrings.AzureStorageConnectionString);

            _cloudTable = new TableClient(ConnectionStrings.AzureStorageConnectionString, typeof(TEntity).Name);

            _cloudTable.CreateIfNotExists();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var execute = await _cloudTable.AddEntityAsync<TEntity>(entity);
            
            return execute.Content as TEntity;
        }

        public IQueryable<TEntity> All()
        {
            return _cloudTable.Query<TEntity>().AsQueryable();
        }

        public async Task DeleteAsync(string rowKey, string partitionKey)
        {
            await _cloudTable.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<TEntity> GetAsync(string rowKey, string partitionKey)
        {
            try
            {
                var execute = await _cloudTable.GetEntityAsync<TEntity>(partitionKey, rowKey);

                return execute.Value;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return _cloudTable.Query<TEntity>().AsQueryable().Where(query);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var execute = await _cloudTable.UpdateEntityAsync(entity, entity.ETag);

            return execute.Content as TEntity;

        }
    }
}
