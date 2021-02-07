using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S4N.LunchToHome.Application.Common;
using S4N.LunchToHome.Domain.Entities;

namespace S4N.LunchToHome.Infrastructure.Persistence
{
    public class MemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly string tableName;

        public MemoryRepository()
        {
            tableName = typeof(T).Name;

            if (!Tables.ContainsKey(tableName))
            {
                Tables.Add(tableName, new List<T>());
            }
        }

        internal static IDictionary<string, IList<T>> Tables = new Dictionary<string, IList<T>>();

        public IQueryable<T> Items
        {
            get
            {
                return Tables[this.tableName].AsQueryable();
            }
        }

        private IList<T> Data
        {
            get
            {
                return Tables[this.tableName];
            }
        }

        public Task DeleteAsync(T entity)
        {
            this.Data.Remove(entity);
            return Task.CompletedTask;
        }

        public Task InsertAsync(T entity)
        {
            this.Data.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            return Task.CompletedTask;
        }
    }
}