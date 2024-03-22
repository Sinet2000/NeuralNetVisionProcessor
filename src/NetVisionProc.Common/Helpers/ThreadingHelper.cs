namespace NetVisionProc.Common.Helpers
{
    public class ThreadingHelper
    {
        public static Task ParallelizeAsync(Func<int, Task> actionAsync, int tasksCount)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < tasksCount; i++)
            {
                var index = i;
                tasks.Add(Task.Run(() => actionAsync(index)));
            }

            return Task.WhenAll(tasks);
        }
    }
}