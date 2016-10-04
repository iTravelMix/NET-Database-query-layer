namespace ADO.Query.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Slapper;

    public class QueryMapper : IQueryMappers, IDisposable
    {
        #region Implementation of IMapper

        public QueryMapper()
        {
            AutoMapper.Cache.ClearInstanceCache();
        }

        public IEnumerable<TDestination> MapDynamicToList<TDestination>(List<object> source) where TDestination : class
        {
            return (source != null) ? AutoMapper.MapDynamic<TDestination>(source) : null;
        }

        public TDestination MapDynamicToSingle<TDestination>(IList<object> source) where TDestination : class
        {
            if (source == null) throw new InvalidOperationException("Query result is empty...");
            return AutoMapper.MapDynamic<TDestination>(source).Single();
        }

        public TDestination MapDynamicToFirstOrDefault<TDestination>(IList<object> source) where TDestination : class
        {
            return source == null ? null : AutoMapper.MapDynamic<TDestination>(source).FirstOrDefault();
        }

        #endregion

        public void Dispose()
        {
            AutoMapper.Cache.ClearInstanceCache();
        }
    }
}
