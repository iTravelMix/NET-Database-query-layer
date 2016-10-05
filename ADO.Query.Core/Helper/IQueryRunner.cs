namespace ADO.Query.Helper
{
    using System.Data;
    using ADO.Query.SqlQuery;

    public interface IQueryRunner
    {
        IDbConnection GetConnection();

        QueryMapperResult<TResult> Execute<TResult>(SqlQuery criterial) where TResult : class;
        PageSqlResult<TResult> Execute<TResult>(SqlPagedQuery criterial) where TResult : class;

        IDataReader ExecuteReader(SqlQuery criterial);
        T ExecuteScalar<T>(SqlQuery criterial);
    }
}