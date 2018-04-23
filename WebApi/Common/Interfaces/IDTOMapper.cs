
using System.Collections.Generic;

namespace WebApi.Common.Interfaces
{
    public interface IDTOMapper
    {
        TResult AutoMapper<TSource, TResult>(TSource sourceModel);
        List<TResult> AutoMapper<TSource, TResult>(List<TSource> sourceModel);
    }
}
