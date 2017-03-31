namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;
    using Dto;
    using SqlQuery;

    public class QueryPhone : SqlQueryGeneric<SimpleDto>
    {
        public QueryPhone()
        {
            this.Expression = "select id as Id, phone as Name from [Phones]";
        }

        public QueryPhone(int id)
        {
            this.Expression = "select id as Id, phone as Name from [Phones] where id = @id";

            this.Parameters = new Dictionary<string, object>
            {
                { nameof(id), id }                                      
            };
        }
    }
}
