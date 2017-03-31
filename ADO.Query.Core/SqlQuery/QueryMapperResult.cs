namespace ADO.Query.SqlQuery
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Mapper;

    public class QueryMapperResult<T> where T : class
    {
        private readonly IQueryMappers mapper;
        private readonly dynamic source;
        private readonly bool keepCache;

        internal QueryMapperResult(IQueryMappers mapper, dynamic source, bool keepCache = true)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper), "Mapper can't be null in QueryMapperResult");

            this.mapper = mapper;
            this.source = source;
            this.keepCache = keepCache;
        }

        public List<T> ToList()
        {
            IEnumerable<T> result = this.mapper.MapDynamicToEnumerable<T>(this.source, keepCache);
            return result.ToList();
        }

        public IEnumerable<T> ToEnumerable()
        {
            return this.mapper.MapDynamicToEnumerable<T>(this.source, keepCache);
        }

        public T ToFirstOrDefault()
        {
            return this.mapper.MapDynamicToFirstOrDefault<T>(this.source, keepCache);
        }

        public T ToSingle()
        {
            return this.mapper.MapDynamicToSingle<T>(this.source, keepCache);
        }

    }
}
