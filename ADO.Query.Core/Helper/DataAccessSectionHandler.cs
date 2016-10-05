
namespace ADO.Query.Helper
{
    using Microsoft.Extensions.Configuration;

    public sealed class DataAccessSectionHandler
    {
        public DataAccessSectionHandler(IConfigurationSection sectionSettings, IConfigurationSection sectionData)
        {
            Type = sectionSettings["Type"];
            ConnectionString = sectionData[sectionSettings["PathConnectionString"]];
        }

        public string Type { get; }
        public string ConnectionString { get; }
    }
}