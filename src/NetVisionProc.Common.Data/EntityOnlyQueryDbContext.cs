using NetVisionProc.Common.Data.Interfaces;

namespace NetVisionProc.Common.Data
{
    public class EntityOnlyQueryDbContext<TContext> : IEntityOnlyQueryDbContext<TContext>
        where TContext : DbContext
    {
        public EntityOnlyQueryDbContext(TContext context, IMapper mapper)
        {
            BindedDbContext = context;
            Mapper = mapper;
        }

        public TContext BindedDbContext { get; init; }
        public IMapper Mapper { get; init; }

        public async Task<T?> GetByIdAsync<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            Guard.Against.NegativeOrZero(id, nameof(id));

            return await List<T>(includes).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TReturn?> GetByIdAsync<T, TReturn>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            var found = await GetByIdAsync<T>(id, includes: includes);
            return Mapper.Map<TReturn>(found);
        }

        public async Task<T> GetSingleByIdAsync<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            Guard.Against.NegativeOrZero(id, nameof(id));

            var found = await GetByIdAsync<T>(id, includes: includes) ?? throw new EntityNotFoundException(typeof(T), nameof(IBaseEntity.Id), id);
            return found!;
        }

        public async Task<TReturn> GetSingleByIdAsync<T, TReturn>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            var found = await GetSingleByIdAsync<T>(id, includes: includes);
            return Mapper.Map<TReturn>(found);
        }

        public async Task<T> GetSingleAsync<T>(Expression<Func<T, bool>> where, string propName, object? findByValue, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            Guard.Against.Null(where, nameof(where));
            var found = await List<T>(includes).FirstOrDefaultAsync(where);

            return found ?? throw new EntityNotFoundException(typeof(T), propName, findByValue);
        }

        public IQueryable<T> List<T>()
            where T : class, IBaseEntity
        {
            return BindedDbContext.Set<T>().AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> List<T>(IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            return List<T>()
                .IncludeNavigationProperties(includes);
        }

        public IEnumerable<TReturn> List<T, TReturn>(IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            var entities = List<T>()
                .IncludeNavigationProperties(includes);

            return Mapper.Map<IEnumerable<TReturn>>(entities);
        }

        public T? GetById<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            Guard.Against.NegativeOrZero(id, nameof(id));

            return List<T>(includes).FirstOrDefault(e => e.Id == id);
        }

        public TReturn? GetById<T, TReturn>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            var found = GetById<T>(id, includes: includes);
            return Mapper.Map<TReturn>(found);
        }

        public T GetSingleById<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            Guard.Against.NegativeOrZero(id, nameof(id));

            var found = GetById<T>(id, includes: includes) ?? throw new EntityNotFoundException(typeof(T), nameof(IBaseEntity.Id), id);
            return found!;
        }

        public T GetSingle<T>(Expression<Func<T, bool>> where, string propName, object? findByValue, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity
        {
            Guard.Against.Null(where, nameof(where));
            var found = List<T>(includes).FirstOrDefault(where);

            return found ?? throw new EntityNotFoundException(typeof(T), propName, findByValue);
        }
    }
}
