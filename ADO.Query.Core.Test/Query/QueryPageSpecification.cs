
namespace ADO.Query.Test.Query
{
    using SqlQuery;

    public class QueryPageSpecification : SqlPagedQuery
    {
        public QueryPageSpecification(int page, int itemsPerPages)
        {
            this.Expression = "select...";
            this.SqlCount = "select count(id)...";

            this.ItemsPerPage = itemsPerPages;
            this.Page = page;
        }
    }
}
