namespace ADO.Query.Helper
{
    using System.Data;
    using ADO.Query.SqlQuery;

    public interface IQueryRunner
    {
        IDbConnection GetConnection();

        QueryMapperResult<TResult> Execute<TResult>(ISqlQuery criterial) where TResult : class;
        PageSqlResult<TResult> Execute<TResult>(ISqlPagedQuery criterial) where TResult : class;

        IDataReader ExecuteReader(ISqlQuery criterial);
        T ExecuteScalar<T>(ISqlQuery criterial);
    }
}