namespace ADO.Query.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Slapper;

    public class QueryMapper : IQueryMappers, IDisposable
    {
        public static bool RemoveCacheAfterExecuteQuery { get; set; }

        #region Implementation of IMapper

        public QueryMapper()
        {
            AutoMapper.Cache.ClearInstanceCache();
        }

        public IEnumerable<TDestination> MapDynamicToEnumerable<TDestination>(List<object> source, bool keepCache = true) where TDestination : class
        {
            return (source != null) ? AutoMapper.MapDynamic<TDestination>(source, keepCache && !RemoveCacheAfterExecuteQuery) : null;
        }

        public TDestination MapDynamicToSingle<TDestination>(IList<object> source, bool keepCache = true) where TDestination : class
        {
            if (source == null) throw new InvalidOperationException("Query result is empty...");
            return AutoMapper.MapDynamic<TDestination>(source, keepCache && !RemoveCacheAfterExecuteQuery).Single();
        }

        public TDestination MapDynamicToFirstOrDefault<TDestination>(IList<object> source, bool keepCache = true) where TDestination : class
        {
            return source == null ? null : AutoMapper.MapDynamic<TDestination>(source, keepCache && !RemoveCacheAfterExecuteQuery).FirstOrDefault();
        }

        #endregion

        public void Dispose()
        {
            AutoMapper.Cache.ClearInstanceCache();
            GC.SuppressFinalize(this);
        }
    }
}
