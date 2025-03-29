using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Applications;

public class TurnApp
{
    TurnService _turnService = new();
    public WriteableBitmap TurnImage(WriteableBitmap bitmap, int angle)
    {
        return _turnService.RotateImage(bitmap, angle);
    }
}