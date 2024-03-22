namespace NetVisionProc.Common.Exceptions
{
    /// <summary>
    /// Exception type for integration query exceptions.
    /// </summary>
    public class IntegrationQueryException : DomainOperationException
    {
        public IntegrationQueryException(string scope, string message, int? statusCode = null)
            : base(BuildContext(scope), BuildMessage(scope, message, statusCode))
        {
        }

        private static string BuildContext(string scope)
        {
            return $"IntegrationQueryException.{scope}";
        }

        private static string BuildMessage(string scope, string message, int? statusCode = null)
        {
            return $"StatusCode: {statusCode}\n" +
                $"Message: {message}\n" +
                $"Scope: {scope}";
        }
    }
}