using Avalonia.Media.Imaging;

namespace ICGFilter.Domain.Filters;

public interface IImageFilter
{
    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap);
}