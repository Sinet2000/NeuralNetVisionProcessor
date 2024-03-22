namespace NetVisionProc.Common.Exceptions
{
    public class DomainOperationException : Exception
    {
        public DomainOperationException(string context, string message)
            : base(message)
        {
            Context = context;
        }

        public DomainOperationException(string context, string message, Exception innerException)
            : base(message, innerException)
        {
            Context = context;
        }

        public string Context { get; init; }
    }
}
