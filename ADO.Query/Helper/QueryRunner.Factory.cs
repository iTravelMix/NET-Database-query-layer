using System;
using System.Collections;
using System.Configuration;
using ADO.Query.Mapper;

namespace ADO.Query.Helper
{
    public abstract partial class QueryRunner
    {
        #region - Factory -

        public static IQueryRunner CreateHelper(string providerAlias)
        {
            return CreateHelper(providerAlias, null);
        }

        public static IQueryRunner CreateHelper(string providerAlias, IQueryMappers mapper)
        {
            try
            {
                var dict = ConfigurationManager.GetSection("DataQueryProviders") as IDictionary;
                if (dict == null) throw new NullReferenceException("Null Reference in DataAccess Provider configuration Session.");

                var providerConfig = dict[providerAlias] as ProviderAlias;
                if (providerConfig == null) throw new NullReferenceException("Null Reference in Provider Alias configuration Session.");

                return CreateHelper(new DataAccessSectionSettings(providerConfig.TypeName, providerConfig.ConnectionString), mapper);
            }
            catch (Exception e)
            {
                throw new Exception("If the section is not defined on the configuration file this method can't be used to create an AdoHelper instance.", e);
            }
        }

        public static IQueryRunner CreateHelper(DataAccessSectionSettings settings)
        {
            return CreateHelper(settings, null);
        }

        public static IQueryRunner CreateHelper(DataAccessSectionSettings settings, IQueryMappers mapper)
        {
            try
            {
                var providerType = settings.Type;
                ConnectionString = settings.ConnectionString;

                var daType = Type.GetType(providerType);
                if (daType == null) throw new NullReferenceException("Null Reference in Provider type configuration Session.");

                var provider = Activator.CreateInstance(daType, mapper);
                if (provider is QueryRunner)
                {
                    return provider as IQueryRunner;
                }

                throw new Exception("The provider specified does not extends the QueryRunner abstract class.");
            }
            catch (Exception e)
            {
                throw new Exception("If the section is not defined on the configuration file this method can't be used to create an QueryRunner instance.", e);
            }
        }

        #endregion
    }
}
