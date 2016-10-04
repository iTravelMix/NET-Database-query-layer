using System;

namespace ADO.Query.SqlQuery
{
    [Obsolete("Inherit directly from SqlPageQuery abstract class to avoid implement this properties in your query classes.")]
    public interface ISqlPagedQuery : ISqlQuery
    {
        string SqlCount { get; }
        int Page { get; }
        int ItemsPerPage { get; }
    }
}
