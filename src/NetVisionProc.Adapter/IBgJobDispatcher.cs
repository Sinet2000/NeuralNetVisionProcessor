using System.Linq.Expressions;

namespace NetVisionProc.Adapter
{
    public interface IBgJobDispatcher
    {
        string Enqueue<T>(Expression<Func<T, Task>> methodCall, string? queue = null);

        string ScheduleDelayed<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay, string? queue = null);

        string ContinueJobOnSuccess<T>(string parentId, Expression<Func<T, Task>> methodCall, bool onlyOnSucceededState = true);
    }
}