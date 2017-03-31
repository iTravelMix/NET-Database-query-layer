using System;
using ADO.Query.Mapper;

namespace ADO.Query.Helper
{
    public abstract partial class QueryRunner
    {
        #region - Factory -

        public static IQueryRunner CreateHelper(DataAccessSectionSettings settings)
        {
            return CreateHelper(settings, null);
        }

        public static IQueryRunner CreateHelper(DataAccessSectionSettings settings, IQueryMappers mapper)
        {
            try
            {
                var providerType = settings.Type;

                var daType = Type.GetType(providerType);
                if (daType == null) throw new NullReferenceException("Null Reference in Provider type configuration Session.");

                var provider = Activator.CreateInstance(daType, settings.ConnectionString, mapper);
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
