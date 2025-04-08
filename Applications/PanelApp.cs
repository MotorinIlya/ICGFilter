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
            _panel.FiltredBitmap = bitmap;
            _panel.ResizeBitmap = bitmap;
            using var buf = bitmap.Lock();
            _panel.Width = buf.Size.Width;
            _panel.Height = buf.Size.Height;
        }
    }

    public (int, int) GetSizeBitmap()
    {
        var bitmap = _panel.OriginalBitmap;
        using var buf = bitmap.Lock();
        return (buf.Size.Width, buf.Size.Height);
    }

    public WriteableBitmap GetOriginalBitmap() => _panel.OriginalBitmap;

    public WriteableBitmap GetBitmap() => _panel.Bitmap;

    public WriteableBitmap GetFiltredBitmap() => _panel.FiltredBitmap;

    public WriteableBitmap GetResizeBitmap() => _panel.ResizeBitmap;

    public void SetFiltredBitmap(WriteableBitmap bitmap) => 
            _panel.FiltredBitmap = bitmap;
    
    public void SetResizeBitmap(WriteableBitmap bitmap) =>
            _panel.ResizeBitmap = bitmap;
}