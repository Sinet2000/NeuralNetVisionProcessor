using System.Linq.Expressions;

namespace NetVisionProc.Adapter
{
    public interface IEntityQueryStore
    {
        IQueryable<T> List<T>()
            where T : class, IBaseEntity;

        IQueryable<T> List<T>(IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        IEnumerable<TReturn> List<T, TReturn>(IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        T? GetById<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        TReturn? GetById<T, TReturn>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        T GetSingleById<T>(int id, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;

        T GetSingle<T>(Expression<Func<T, bool>> where, string propName, object? findByValue, IEnumerable<string>? includes = null)
            where T : class, IBaseEntity;
    }
}
