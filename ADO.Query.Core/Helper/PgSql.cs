namespace ADO.Query.Helper
{
    using System;
    using System.Data;

    using ADO.Query.Mapper;

    using Npgsql;

    /// <summary>
    /// Clase que hereda de AdoHelper e implementa los métodos necesarios para
    /// trabajar con Postgre como servidor de datos.
    /// </summary>
    public class PgSql : QueryRunner
    {
        public PgSql(IQueryMappers mapper) : base(mapper)
        {
        }

        public override IDbConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new NullReferenceException("ConnectionString");
            return new NpgsqlConnection(ConnectionString);
        }

        /// <summary>
        /// Permite retornar una conexión a la base de datos a partir de una cadena de conexión pasada como parámetro
        /// </summary>
        /// <param name="connectionString">cadena de conexión a utilizar</param>
        /// <returns>Una conexión a la base de datos</returns>
        public IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new NullReferenceException("connectionString");
            return new NpgsqlConnection(connectionString);
        }

        protected override IDataParameter GetParameter()
        {
            return new NpgsqlParameter();
        }
    }
}
