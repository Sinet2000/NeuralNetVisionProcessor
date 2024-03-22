using Microsoft.EntityFrameworkCore.Storage;
using NetVisionProc.Common.Data.Models;
using Sieve.Models;

namespace NetVisionProc.Common.Data.Interfaces
{
    public interface IEntityDbContext<TContext> : IEntityOnlyQueryDbContext<TContext>
        where TContext : DbContext
    {
        Task<PagedResponse<TReturn>> List<T, TReturn>(
            SieveModel request,
            IEnumerable<string>? includes = null,
            Expression<Func<T, bool>>? where = null,
            CancellationToken cancellationToken = default)
            where T : class, IBaseEntity
            where TReturn : class;

        IQueryable<T> ListAsTracking<T>(IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task<T?> GetByIdAsTracking<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task<T> GetSingleByIdAsTracking<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task Delete<T>(int id, bool check = false)
            where T : class, IBaseEntity;

        Task Update<T>(object dto, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task Update<T>(int id, Action<T> update, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task Update<T>(int id, Func<T, Task> update, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task<TReturn> UpdateAndReturn<T, TReturn>(object dto, Action<T>? update = null, bool setModifiedExplicitly = false, IEnumerable<string>? includes = null, IEnumerable<string>? includesForReturn = null)
            where T : class, IBaseEntity
            where TReturn : class;

        Task<TReturn> UpdateAndReturn<T, TReturn>(int id, Action<T> update, bool setModifiedExplicitly = false, IEnumerable<string>? includes = null, IEnumerable<string>? includesForReturn = null)
            where T : class, IBaseEntity
            where TReturn : class;

        Task<TReturn> UpdateAndReturn<T, TReturn>(int id, Func<T, Task> update, bool setModifiedExplicitly = false, IEnumerable<string>? includes = null, IEnumerable<string>? includesForReturn = null)
            where T : class, IBaseEntity
            where TReturn : class;

        Task CreateRangeInTransaction<T>(IEnumerable<T> entities)
            where T : class, IBaseEntity;

        Task<int> Create<T>(T toCreate)
            where T : class, IBaseEntity;

        Task<T> CreateAndReturn<T>(object dto)
            where T : class, IBaseEntity;

        Task<TReturn> CreateAndReturn<T, TReturn>(object dto, Action<T>? modify = null, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
            where TReturn : class;

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
