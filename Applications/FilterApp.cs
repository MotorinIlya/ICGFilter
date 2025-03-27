using Avalonia.Media.Imaging;
using ICGFilter.Domain.Repository;

namespace ICGFilter.Applications;

public class FilterApp
{
    private FilterRepository _filters = new();
    public WriteableBitmap SetInverseFilter(WriteableBitmap bitmap)
    {
        var filter = _filters.GetFilter(FilterName.Inversion);
        return filter.Apply(bitmap);
    }
}