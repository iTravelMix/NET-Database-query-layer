namespace ADO.Query.Helper
{
    using System.Configuration;

    public class ConfigurationMissingException : ConfigurationErrorsException
    {
        public ConfigurationMissingException() : base("The 'daProvider' node must contain required attributes.")
        {
        }

        public ConfigurationMissingException(string property) : base(string.Format("The 'daProvider' node must contain an attribute named '{0}'.", property)) 
        {
        }
    }
}
