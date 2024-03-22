using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;
using NetVisionProc.Common.Data.Interfaces;
using NetVisionProc.Common.Data.Models;
using Sieve.Models;
using Sieve.Services;

namespace NetVisionProc.Common.Data
{
    public class EntityDbContext<TContext> : EntityOnlyQueryDbContext<TContext>, IEntityDbContext<TContext>
        where TContext : DbContext
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo> _idPropertyInfoCache = new();

        private readonly ISieveProcessor _search;

        public EntityDbContext(TContext context, IMapper mapper, ISieveProcessor search)
            : base(context, mapper)
        {
            _search = search;
        }

        public async Task<PagedResponse<TReturn>> List<T, TReturn>(SieveModel request, IEnumerable<string>? includes, Expression<Func<T, bool>>? where, CancellationToken cancellationToken)
            where T : class, IBaseEntity
            where TReturn : class
        {
            var query = List<T>(includes);

            if (where is not null)
            {
                query = query.Where(where);
            }

            query = _search.Apply(request, query, applySorting: false, applyFiltering: true, applyPagination: false);
            int totalCount = await query.CountAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Sorts))
            {
                request.Sorts = CommonConst.MainColumnKey;
            }

            query = _search.Apply(request, query, applySorting: true, applyFiltering: false, applyPagination: true);

            var list = await query.ToListAsync(cancellationToken);

            return new PagedResponse<TReturn>(request, totalCount)
            {
                Data = Mapper.Map<List<TReturn>>(list)
            };
        }

        public IQueryable<T> ListAsTracking<T>(IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            return BindedDbContext.Set<T>().AsQueryable().IncludeNavigationProperties(includes).AsTracking();
        }

        public async Task<T?> GetByIdAsTracking<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            return await ListAsTracking<T>(includes)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<T> GetSingleByIdAsTracking<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            var found = await GetByIdAsTracking<T>(id, includes: includes) ?? throw new EntityNotFoundException(typeof(T), nameof(IBaseEntity.Id), id);
            return found!;
        }

        public async Task Delete<T>(int id, bool check = false)
            where T : class, IBaseEntity
        {
            var found = await BindedDbContext.FindAsync<T>(id);
            if (check && found is null)
            {
                throw new EntityNotFoundException(typeof(T), nameof(IBaseEntity.Id), id);
            }

            if (found is not null)
            {
                try
                {
                    BindedDbContext.Remove(found);
                    await BindedDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task Update<T>(object dto, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            int id = ExtractEntityId(dto);
            await Update<T>(
                id,
                toUpdate => BindedDbContext.Entry(toUpdate).CurrentValues.SetValues(dto),
                includes: includes);
        }

        public async Task Update<T>(int id, Action<T> update, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            var toUpdate = await GetSingleByIdAsTracking<T>(id, includes);
            update(toUpdate);
            await BindedDbContext.SaveChangesAsync();
        }

        public async Task Update<T>(int id, Func<T, Task> update, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            var toUpdate = await GetSingleByIdAsTracking<T>(id, includes);
            await update(toUpdate);
            await BindedDbContext.SaveChangesAsync();
        }

        public async Task<TReturn> UpdateAndReturn<T, TReturn>(
            object dto,
            Action<T>? update = null,
            bool setModifiedExplicitly = false,
            IEnumerable<string>? includes = null,
            IEnumerable<string>? includesForReturn = null)
            where T : class, IBaseEntity
            where TReturn : class
        {
            int id = ExtractEntityId(dto);

            return await UpdateAndReturn<T, TReturn>(
                id,
                toUpdate =>
                {
                    BindedDbContext.Entry(toUpdate).CurrentValues.SetValues(dto);
                    update?.Invoke(toUpdate);
                },
                setModifiedExplicitly: setModifiedExplicitly,
                includes: includes,
                includesForReturn: includesForReturn);
        }

        public async Task<TReturn> UpdateAndReturn<T, TReturn>(
            int id,
            Action<T> update,
            bool setModifiedExplicitly = false,
            IEnumerable<string>? includes = null,
            IEnumerable<string>? includesForReturn = null)
            where T : class, IBaseEntity
            where TReturn : class
        {
            var toUpdate = await GetSingleByIdAsTracking<T>(id, includes);

            update(toUpdate);

            if (setModifiedExplicitly)
            {
                BindedDbContext.Entry(toUpdate).State = EntityState.Modified;
            }

            await BindedDbContext.SaveChangesAsync();

            if (includesForReturn.HasValue())
            {
                return await GetSingleByIdAsync<T, TReturn>(toUpdate.Id, includes: includesForReturn);
            }

            return Mapper.Map<TReturn>(toUpdate);
        }

        public async Task<TReturn> UpdateAndReturn<T, TReturn>(
            int id,
            Func<T, Task> update,
            bool setModifiedExplicitly = false,
            IEnumerable<string>? includes = null,
            IEnumerable<string>? includesForReturn = null)
            where T : class, IBaseEntity
            where TReturn : class
        {
            var toUpdate = await GetSingleByIdAsTracking<T>(id, includes);

            await update(toUpdate);

            if (setModifiedExplicitly)
            {
                BindedDbContext.Entry(toUpdate).State = EntityState.Modified;
            }

            await BindedDbContext.SaveChangesAsync();

            if (includesForReturn.HasValue())
            {
                return await GetSingleByIdAsync<T, TReturn>(toUpdate.Id, includes: includesForReturn);
            }

            return Mapper.Map<TReturn>(toUpdate);
        }

        public async Task CreateRangeInTransaction<T>(IEnumerable<T> entities)
            where T : class, IBaseEntity
        {
            using (var tran = await BindedDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    BindedDbContext.Set<T>().AddRange(entities);
                    await BindedDbContext.SaveChangesAsync();

                    await tran.CommitAsync();
                }
                catch (Exception)
                {
                    await tran.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<int> Create<T>(T toCreate)
            where T : class, IBaseEntity
        {
            BindedDbContext.Set<T>().Add(toCreate);
            await BindedDbContext.SaveChangesAsync();

            return toCreate.Id;
        }

        public async Task<T> CreateAndReturn<T>(object dto)
            where T : class, IBaseEntity
        {
            T toCreate = Mapper.Map<T>(dto);

            BindedDbContext.Set<T>().Add(toCreate);
            await BindedDbContext.SaveChangesAsync();
            return toCreate;
        }

        public async Task<TReturn> CreateAndReturn<T, TReturn>(object dto, Action<T>? modify = null, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
            where TReturn : class
        {
            T toCreate = Mapper.Map<T>(dto);

            modify?.Invoke(toCreate);

            BindedDbContext.Set<T>().Add(toCreate);
            await BindedDbContext.SaveChangesAsync();

            if (includes.HasValue())
            {
                return (await GetByIdAsync<T, TReturn>(toCreate.Id, includes: includes))!;
            }

            return Mapper.Map<TReturn>(toCreate);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await BindedDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

        private static int ExtractEntityId(object entityDto)
        {
            var entityType = entityDto.GetType();
            var idProperty = _idPropertyInfoCache.GetOrAdd(entityType, GetIdPropertyInfo);
            Guard.Against.Null(idProperty, nameof(idProperty));
            var idValue = idProperty.GetValue(entityDto);
            Guard.Against.Null(idValue, nameof(idValue));

            return (int)idValue!;
        }

        private static PropertyInfo GetIdPropertyInfo(Type type)
        {
            var idProp = type.GetProperty(nameof(IBaseEntity.Id));
            Guard.Against.Null(idProp, nameof(idProp));

            return idProp;
        }
    }
}
