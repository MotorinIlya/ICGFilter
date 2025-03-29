using Avalonia.Media.Imaging;
using ICGFilter.Presentation.Views;

namespace ICGFilter.Applications;

public class PanelApp(PhotoPanel panel)
{
    private PhotoPanel _panel = panel;
    public PhotoPanel Panel => _panel;
    public void ChangeBitmap(WriteableBitmap? bitmap)
    {
        if (bitmap is not null)
        {
            _panel.Bitmap = bitmap;
            using var buf = bitmap.Lock();
            _panel.Width = buf.Size.Width;
            _panel.Height = buf.Size.Height;
        }
    }

    public void ChangeAllBitmap(WriteableBitmap? bitmap)
    {
        if (bitmap is not null)
        {
            _panel.Bitmap = bitmap;
            _panel.OriginalBitmap = bitmap;
            using var buf = bitmap.Lock();
            _panel.Width = buf.Size.Width;
            _panel.Height = buf.Size.Height;
        }
    }

    public WriteableBitmap GetOriginalBitmap()
    {
        return _panel.OriginalBitmap;
    }

    public WriteableBitmap GetBitmap()
    {
        return _panel.Bitmap;
    }
}