namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;

    using SqlQuery;

    class QueryWithParameters : SqlQuery
    {
        public QueryWithParameters(int id, string name)
        {
            this.Expression = "select id as Id, name as Name from table_in_database where id = :id and name = :name";

            this.Parameters = new Dictionary<string, object>
            {
                {"id", id},
                {"name", name}
            };
        }
    }
}
