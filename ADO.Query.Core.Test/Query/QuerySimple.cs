namespace ADO.Query.Test.Query
{
    using SqlQuery;

    class QuerySimple : SqlQuery
    {
        public QuerySimple()
        {
            this.Expression = "select id as Id, name as Name from table_in_database";
        }
    }
}
