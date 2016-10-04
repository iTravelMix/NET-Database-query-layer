namespace ADO.Query.Mapper
{
    using System.Collections.Generic;

    public interface IQueryMappers
    {
        IEnumerable<TDestination> MapDynamicToList<TDestination>(List<object> source) where TDestination : class;
        TDestination MapDynamicToSingle<TDestination>(IList<object> source) where TDestination : class;
        TDestination MapDynamicToFirstOrDefault<TDestination>(IList<object> source) where TDestination : class;
    }
}
