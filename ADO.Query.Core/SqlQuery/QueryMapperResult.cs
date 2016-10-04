namespace ADO.Query.SqlQuery
{
    using System;
    using System.Collections.Generic;
    using ADO.Query.Mapper;

    public class QueryMapperResult<T> where T : class
    {
        private readonly IQueryMappers mapper;
        private readonly dynamic source;

        public QueryMapperResult(IQueryMappers mapper, dynamic source)
        {
            if (mapper == null) throw new ArgumentNullException("mapper", "Mapper can't be null in QueryMapperResult");

            this.mapper = mapper;
            this.source = source;
        }

        public IEnumerable<T> ToList()
        {
            return this.mapper.MapDynamicToList<T>(this.source);
        }

        public T ToFirstOrDefault()
        {
            return this.mapper.MapDynamicToFirstOrDefault<T>(this.source);
        }

        public T ToSingle()
        {
            return this.mapper.MapDynamicToSingle<T>(this.source);
        }

    }
}
