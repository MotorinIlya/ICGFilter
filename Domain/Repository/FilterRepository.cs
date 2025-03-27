using System.Collections.Generic;
using ICGFilter.Domain.Filters;

namespace ICGFilter.Domain.Repository;

public class FilterRepository
{
    private readonly Dictionary<FilterName, IImageFilter> _filters = new()
    {
        {FilterName.Inversion, new InversionFilter()},
    };

    public IImageFilter GetFilter(FilterName filterName) => _filters[filterName];
}