using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ICGFilter.Presentation.Views;

public class PhotoPanel : UserControl
{
    private WriteableBitmap _bitmap;
    public WriteableBitmap Bitmap 
    {
        get => _bitmap;
        set
        {
            _bitmap = value;
            InvalidateVisual();
        }
    }

    public PhotoPanel()
    {
        _bitmap = new(
            new PixelSize(10, 10),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.DrawImage(_bitmap, 
                new Rect(0, 0, _bitmap.PixelSize.Width, 
                _bitmap.PixelSize.Height));
    }

    public unsafe void SetBitmap(int width, int height)
    {
        Width = width;
        Height = height;
        _bitmap = new(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);

        using var buffer = _bitmap.Lock();
        var ptr = (byte*)buffer.Address.ToPointer();
        for (int i = 0; i < buffer.Size.Width * buffer.Size.Height * 4; i += 4)
        {
            ptr[i] = 255;
            ptr[i + 1] = 255;
            ptr[i + 2] = 255;
            ptr[i + 3] = 255;
        }
    }
}