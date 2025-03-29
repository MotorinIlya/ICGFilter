using Avalonia.Media.Imaging;
using ICGFilter.Domain.Repository;

namespace ICGFilter.Applications;

public class FilterApp
{
    private FilterRepository _filters = new();

    public WriteableBitmap GetFilter(WriteableBitmap bitmap, FilterName filterName)
    {
        var filter = _filters.GetFilter(filterName);
        return filter.Apply(bitmap);
    }
}