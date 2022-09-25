using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorage.Library
{
    public interface INoSqlStorage<TEntity> 
    {
        Task<TEntity> AddAsync(TEntity entity);

        Task DeleteAsync(string rowKey, string partitionKey);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task<TEntity> GetAsync(string rowKey, string partitionKey);

        IQueryable<TEntity> All();

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query);

    }
}
