namespace NetVisionProc.Common.Data.Interfaces
{
    public interface IEntityOnlyQueryDbContext<TContext> : IEntityQueryStore
        where TContext : DbContext
    {
        TContext BindedDbContext { get; init; }

        IMapper Mapper { get; init; }

        Task<T?> GetByIdAsync<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task<TReturn?> GetByIdAsync<T, TReturn>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task<T> GetSingleByIdAsync<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task<TReturn> GetSingleByIdAsync<T, TReturn>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        Task<T> GetSingleAsync<T>(Expression<Func<T, bool>> where, string propName, object? findByValue, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;
    }
}
