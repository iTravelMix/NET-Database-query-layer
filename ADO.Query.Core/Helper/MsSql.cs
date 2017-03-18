
namespace ADO.Query.Helper
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using ADO.Query.Mapper;

    public class MsSql : QueryRunner
    {
        public MsSql(string connectionString, IQueryMappers mapper = null) : base(connectionString, mapper)
        {
        }

        public override IDbConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new NullReferenceException(nameof(ConnectionString));
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Return a database connection using connection string passed as parameter
        /// </summary>
        /// <param name="connectionString">connection string to use</param>
        /// <returns>Database connection</returns>
        public static IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new NullReferenceException(nameof(connectionString));
            return new SqlConnection(connectionString);
        }

        protected override IDataParameter GetParameter()
        {
            return new SqlParameter();
        }
    }
}
