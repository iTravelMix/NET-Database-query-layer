namespace ADO.Query.Test.Query
{
    using SqlQuery;

    public class QueryOneToMany : SqlQuery
    {
        public QueryOneToMany()
        {
            this.Expression = @"select u.id as Id, u.name as Name, p.id as Phones_Id, p.phone as Phones_Phone 
                                    from [user] u left join [phones] p on (p.usid = u.id)";
        }
    }
}
