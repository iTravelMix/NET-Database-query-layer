namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;
    using Dto;
    using SqlQuery;

    public class QueryUsers : SqlQueryGeneric<SimpleDto>
    {
        public QueryUsers()
        {
            this.Expression = "select id as Id, name as Name from [User]";
        }

        public QueryUsers(int id)
        {
            this.Expression = "select id as Id, name as Name from [User] where id = @id";

            this.Parameters = new Dictionary<string, object>
            {
                {nameof(id), id}                                      
            };
        }
    }
}
