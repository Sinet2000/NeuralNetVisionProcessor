namespace NetVisionProc.Common.Exceptions
{
    public class EntityNotFoundException : DomainOperationException
    {
        public EntityNotFoundException(Type entityType, string propertyName, object? searchValue)
            : base(BuildContext(entityType, propertyName), BuildMessage(entityType, propertyName, searchValue))
        {
        }

        private static string BuildContext(Type entityType, string propertyName)
        {
            string entityName = entityType.Name;
            return $"EntityNotFound.{entityName}.{propertyName}";
        }

        private static string BuildMessage(Type entityType, string propertyName, object? searchValue)
        {
            string entityName = entityType.Name;
            return $"{entityName} not found [{propertyName}={searchValue}]";
        }
    }
}
