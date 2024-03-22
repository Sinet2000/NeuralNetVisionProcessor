using System.Collections.Concurrent;

namespace NetVisionProc.Common
{
    public class AsyncLocker
    {
        // Default timeout value in seconds.
        private const int DefaultTimeoutSeconds = 500;
        private const int MaxConcurrency = 10;

        // Dictionary to store SemaphoreSlim instances for different locking contexts.
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> LocksDictionary = new();

        /// <summary>
        /// Locks the resource associated with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of resource to lock.</typeparam>
        /// <param name="timeoutSeconds">Optional. The timeout value in seconds. Default is 1 second.</param>
        /// <returns>A SemaphoreLock instance.</returns>
        public static async Task<SemaphoreLock> LockForResource<T>(double timeoutSeconds = 100)
        {
            try
            {
                // Calls the LockForResource method with the name of the specified type.
                return await LockForResource(typeof(T).Name, timeoutSeconds);
            }
            catch (SemaphoreFullException ex)
            {
                throw new SemaphoreFullException($"Error acquiring lock for resource '{typeof(T).Name}': {ex.Message}");
            }
        }
        
        /// <summary>
        /// Locks the resource associated with the specified key.
        /// </summary>
        /// <param name="resourceKey">The key representing the resource to lock.</param>
        /// <param name="timeoutInSeconds">Optional. The timeout value in seconds. Default is 5 seconds.</param>
        /// <returns>A SemaphoreLock instance.</returns>
        public static async Task<SemaphoreLock> LockForResource(string resourceKey, double timeoutInSeconds = DefaultTimeoutSeconds)
        {
            // Retrieves the SemaphoreSlim instance associated with the resource key.
            var semaphore = LocksDictionary.GetOrAdd(resourceKey, (_) => new SemaphoreSlim(MaxConcurrency));

            // Attempts to acquire the lock asynchronously with the specified timeout.
            if (await semaphore.WaitAsync(TimeSpan.FromSeconds(timeoutInSeconds)))
            {
                // If the lock is acquired successfully, returns a SemaphoreLock instance.
                return new SemaphoreLock(semaphore);
            }

            // If the lock acquisition times out, throws a TimeoutException.
            throw new TimeoutException($"Operation: '{resourceKey}' timed out.");
        }

        // This struct provides a convenient way to release a SemaphoreSlim lock
        // when used in a using statement, ensuring the lock is released even if
        // an exception is thrown.
        public readonly struct SemaphoreLock : IDisposable
        {
            // The SemaphoreSlim instance that will be released when Dispose is called.
            private readonly SemaphoreSlim _semaphore;

            public SemaphoreLock(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
            }

            public void Dispose()
            {
                ReleaseLock();
            }
            
            private void ReleaseLock()
            {
                try
                {
                    _semaphore.Release();
                }
                catch (SemaphoreFullException ex)
                {
                    throw new SemaphoreFullException($"Error releasing lock: {ex.Message}");
                }
            }
        }
    }
}