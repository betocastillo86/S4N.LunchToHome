using System.Linq;
using System.Threading.Tasks;
using S4N.LunchToHome.Domain.Entities;

namespace S4N.LunchToHome.Application.Common
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> Items { get; }

        Task DeleteAsync(T entity);

        Task InsertAsync(T entity);

        Task UpdateAsync(T entity);
    }
}