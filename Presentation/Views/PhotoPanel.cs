using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ICGFilter.Domain.Repository;

namespace ICGFilter.Presentation.Views;

public class PhotoPanel : UserControl
{
    private WriteableBitmap _bitmap;
    private WriteableBitmap _originalBitmap;
    private ScrollRepository _scroll;
    public WriteableBitmap Bitmap 
    {
        get => _bitmap;
        set
        {
            _bitmap = value;
            InvalidateVisual();
        }
    }
    public WriteableBitmap OriginalBitmap
    {
        get => _originalBitmap;
        set
        {
            _bitmap = value;
            _originalBitmap = value;
            InvalidateVisual();
        }
    }

    public PhotoPanel(ScrollViewer scroll)
    {
        _bitmap = new(
            new PixelSize(10, 10),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);
        _originalBitmap = _bitmap;
        _scroll = new(new(0, 0), scroll);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.DrawImage(_bitmap, 
                new Rect(0, 0, _bitmap.PixelSize.Width, 
                _bitmap.PixelSize.Height));
    
            var pen = new Pen
            {
                Brush = Brushes.Red,
                Thickness = 2,
                DashStyle = new DashStyle(new[] { 10.0, 10.0 }, 0)
            };

            var borderRect = new Rect(0, 0, Bounds.Width, Bounds.Height);
            context.DrawRectangle(null, pen, borderRect);
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
        _originalBitmap = _bitmap;
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

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _scroll.LastPressedPoint = e.GetPosition(_scroll.ScrollViewer);
            _scroll.IsPanning = true;
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        
        if (_scroll.IsPanning && e.Pointer.Captured == this)
        {
            var currentPosition = e.GetPosition(_scroll.ScrollViewer);
            var offset = _scroll.ScrollViewer.Offset;
            
            _scroll.ScrollViewer.Offset = new Vector(
                offset.X + (_scroll.LastPressedPoint.X - currentPosition.X),
                offset.Y + (_scroll.LastPressedPoint.Y - currentPosition.Y));
                
            _scroll.LastPressedPoint = currentPosition;
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        
        if (_scroll.IsPanning && e.Pointer.Captured == this)
        {
            _scroll.IsPanning = false;
            e.Pointer.Capture(null);
            e.Handled = true;
        }
    }
}