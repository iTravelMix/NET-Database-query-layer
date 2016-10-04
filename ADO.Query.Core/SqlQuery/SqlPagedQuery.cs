using System.Collections.Generic;

namespace ADO.Query.SqlQuery
{
    public abstract class SqlPagedQuery: ISqlPagedQuery
    {
        public string Expression { get; protected set; }
        public IDictionary<string, object> Parameters { get; protected set; }
        public string SqlCount { get; protected set; }
        public int Page { get; protected set; }
        public int ItemsPerPage { get; protected set; }
    }
}
