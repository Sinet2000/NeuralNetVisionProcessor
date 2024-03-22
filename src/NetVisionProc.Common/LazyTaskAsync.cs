namespace NetVisionProc.Common
{
    public class LazyTaskAsync<T> : Lazy<Task<T>>
    {
        public LazyTaskAsync(Func<T> function)
            : base(() => Task.Factory.StartNew(function))
        {
        }

        public LazyTaskAsync(Func<Task<T>> function)
            : base(() => Task.Factory.StartNew(function).Unwrap())
        {
        }
    }
}