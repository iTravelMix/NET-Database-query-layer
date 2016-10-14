
namespace ADO.Query.Helper
{
    public sealed class DataAccessSectionHandler
    {
        public DataAccessSectionHandler(string type, string connectionString)
        {
            Type = type;
            ConnectionString = connectionString;
        }

        public string Type { get; }
        public string ConnectionString { get; }
    }
}