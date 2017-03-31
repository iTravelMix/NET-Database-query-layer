namespace ADO.Query.Test.Query
{
    using SqlQuery;
    using System.Collections.Generic;

    public class QueryOneToManyMapObject : SqlQuery
    {
        public QueryOneToManyMapObject(int id)
        {
            this.Expression = @"select u.id as Id, u.name as Name, p.id as Phones_Id, p.phone as Phones_Name, e.id as Emails_Id, e.email as Emails_Name 
                                    from [user] u left join [phones] p on (p.usid = u.id) left join [emails] e on (e.usid = u.id) where u.id = @id";

            this.Parameters = new Dictionary<string, object>
            {
                { nameof(id), id }
            };
        }
    }
}
