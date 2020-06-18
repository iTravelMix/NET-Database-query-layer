using System.Collections.Generic;

namespace ADO.Query.SqlQuery
{
    public abstract class SqlPagedQuery: SqlQuery
    {
        public string SqlCount { get; protected set; }
        public int Page { get; protected set; }
        public int ItemsPerPage { get; protected set; }
    }
}
