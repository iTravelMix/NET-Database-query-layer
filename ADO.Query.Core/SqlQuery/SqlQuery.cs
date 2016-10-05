using System.Collections.Generic;

namespace ADO.Query.SqlQuery
{
    public abstract class SqlQuery
    {
        public string Expression { get; protected set; }
        public IDictionary<string, object> Parameters { get; protected set; }
    }
}
