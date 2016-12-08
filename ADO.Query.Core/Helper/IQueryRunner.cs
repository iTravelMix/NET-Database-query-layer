namespace ADO.Query.Helper
{
    using System.Data;
    using ADO.Query.SqlQuery;

    public interface IQueryRunner
    {
        IDbConnection GetConnection();

        QueryMapperResult<TResult> Execute<TResult>(SqlQuery criterial, bool keepCache = true) where TResult : class;
        QueryMapperResult<T> Execute<T>(SqlQueryGeneric<T> criterial, bool keepCache = true) where T : class;


        PageSqlResult<TResult> Execute<TResult>(SqlPagedQuery criterial, bool keepCache = true) where TResult : class;
        PageSqlResult<T> Execute<T>(SqlPagedQueryGeneric<T> criterial, bool keepCache = true) where T : class;

        IDataReader ExecuteReader(SqlQuery criterial);
        T ExecuteScalar<T>(SqlQuery criterial);
    }
}