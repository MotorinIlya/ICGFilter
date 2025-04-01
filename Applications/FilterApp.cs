using Avalonia.Media.Imaging;
using ICGFilter.Domain.Filters;
using ICGFilter.Domain.Repository;

namespace ICGFilter.Applications;

public class FilterApp
{
    private FilterRepository _filters = new();

    public WriteableBitmap ApplyFilter(WriteableBitmap bitmap, FilterName filterName)
    {
        var filter = _filters.GetFilter(filterName);
        return filter.Apply(bitmap);
    }

    public IImageFilter GetFilter(FilterName filterName) => _filters.GetFilter(filterName);
}