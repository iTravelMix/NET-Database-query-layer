namespace ADO.Query.Helper
{
    using System;
    using System.Data;

    using Mapper;
    using Npgsql;

    /// <summary>
    /// Clase que hereda de AdoHelper e implementa los métodos necesarios para
    /// trabajar con Postgre como servidor de datos.
    /// </summary>
    public class PgSql : QueryRunner
    {
        public PgSql(string connectionString, IQueryMappers mapper=null) : base(connectionString, mapper)
        {
        }

        public override IDbConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new NullReferenceException(nameof(ConnectionString));
            return new NpgsqlConnection(ConnectionString);
        }

        /// <summary>
        /// Permite retornar una conexión a la base de datos a partir de una cadena de conexión pasada como parámetro
        /// </summary>
        /// <param name="connectionString">cadena de conexión a utilizar</param>
        /// <returns>Una conexión a la base de datos</returns>
        public static IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new NullReferenceException(nameof(connectionString));
            return new NpgsqlConnection(connectionString);
        }

        protected override IDataParameter GetParameter()
        {
            return new NpgsqlParameter();
        }
    }
}
