namespace ClientApp.Settings
{
    public class AzureTableSettings(string connectionString, string tableName)
    {
        public string TableName {
            get;
        } = tableName;

        public string ConnectionString {
            get;
            set;
        } = connectionString;
    }
}