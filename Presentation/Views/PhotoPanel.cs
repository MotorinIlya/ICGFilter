using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ICGFilter.Domain.Repository;
using ICGFilter.Domain.Services;

namespace ICGFilter.Presentation.Views;

public class PhotoPanel : UserControl
{
    private WriteableBitmap _bitmap;
    private WriteableBitmap _resizeBitmap;
    private WriteableBitmap _originalBitmap;
    private WriteableBitmap _filtredBitmap;
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
        set => _originalBitmap = value;
    }
    public WriteableBitmap FiltredBitmap
    {
        get => _filtredBitmap; 
        set => _filtredBitmap = value;
    }

    public WriteableBitmap ResizeBitmap
    {
        get => _resizeBitmap; 
        set => _resizeBitmap = value;
    }

    public PhotoPanel(ScrollViewer scroll)
    {
        _bitmap = BitmapService.CreateBitmap(10, 10);
        _originalBitmap = _bitmap;
        _filtredBitmap = _bitmap;
        _resizeBitmap = _bitmap;
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

    public void SetBitmap(int width, int height)
    {
        Width = width;
        Height = height;

        _bitmap = BitmapService.CreateBitmap(width, height);
        BitmapService.FillBitmapWhite(_bitmap);
        _originalBitmap = _bitmap;
        _filtredBitmap = _bitmap;
        _resizeBitmap = _bitmap;
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