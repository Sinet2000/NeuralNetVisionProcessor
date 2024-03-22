using Microsoft.Extensions.Logging;

namespace NetVisionProc.Data.Extensions
{
    public static class LoggerExt
    {
        public static void LogTimeElapsed(this ILogger logger, string source, TimeSpan timeElapsed)
        {
            logger.LogInformation("{Source} Finished - {Elapsed}", source, timeElapsed);
        }
    }
}