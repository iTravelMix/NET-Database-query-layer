namespace ADO.Query.SqlQuery
{
    using System.Collections.Generic;

    public class PageSqlResult<T>
    {
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }
        public long CurrentPage { get; set; }

        public IEnumerable<T> Result { get; set; }
    }
}
