using System;

namespace ADO.Query.SqlQuery
{
    using System.Collections.Generic;

    [Obsolete("Inherit directly from SqlQuery abstract class to avoid implement properties in your query classes.")]
    public interface ISqlQuery
    {
        string Expression { get; }
        IDictionary<string, object> Parameters { get; }
    }
}
