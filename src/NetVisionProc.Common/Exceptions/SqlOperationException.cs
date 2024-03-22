namespace NetVisionProc.Common.Exceptions
{
    public class SqlOperationException : Exception
    {
        public SqlOperationException(string query, string message)
            : base(message)
        {
            Query = query;
        }

        public SqlOperationException(string query, string message, Exception innerException)
            : base(message, innerException)
        {
            Query = query;
        }

        public string Query { get; init; }
    }
}