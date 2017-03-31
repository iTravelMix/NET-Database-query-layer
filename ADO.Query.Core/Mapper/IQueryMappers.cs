namespace ADO.Query.Mapper
{
    using System.Collections.Generic;

    public interface IQueryMappers
    {
        IEnumerable<TDestination> MapDynamicToEnumerable<TDestination>(List<object> source, bool keepCache = true) where TDestination : class;
        TDestination MapDynamicToSingle<TDestination>(IList<object> source, bool keepCache = true) where TDestination : class;
        TDestination MapDynamicToFirstOrDefault<TDestination>(IList<object> source, bool keepCache = true) where TDestination : class;
    }
}
