using System;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class RobertsFilter : IImageFilter
{
    private int[,] _hKernel =
    {
        {1, 0},
        {0, -1}
    };

    private int[,] _vKernel =
    {
        {0, 1},
        {-1, 0}
    };
    private BlackWhiteFilter _filter = new();
    private int _threshold = 100;
    public int Threshold
    {
        get => _threshold;
        set
        {
            if (value > 0 && value < 255)
            {
                _threshold = value;
            }
        }
    }

    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        bitmap = _filter.Apply(bitmap);
        using var buf = bitmap.Lock();
        var newBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = newBitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();

        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                var gx = MulToKernel(ptr, _hKernel, x, y, 
                        buf.Size.Width, buf.Size.Height, buf.RowBytes);
                
                var gy = MulToKernel(ptr, _vKernel, x, y, 
                        buf.Size.Width, buf.Size.Height, buf.RowBytes);
                
                var g = Math.Sqrt(gx * gx + gy * gy);
                g = (g > _threshold) ? 255 : 0;

                var origOffset = BitmapService.GetOffset(x, y, newBuf.RowBytes);
                ColorService.SetColor(newPtr, origOffset, (byte)g, (byte)g, (byte)g);
            }
        }
        return newBitmap;
    }

    private unsafe int MulToKernel(
            byte* ptr, int[,] kernel, 
            int x, int y, 
            int width, int height, int rowBytes)
    {
        var result = 0;
        for (var kx = 0; kx <= 1; kx++)
        {
            for (var ky = 0; ky <= 1; ky++)
            {
                var px = x + kx;
                var py = y + ky;
                if (px < 0 || py < 0 || px >= width || py >= height)
                {
                    continue;
                }

                var offset = BitmapService.GetOffset(px, py, rowBytes);
                var kernalValue = _hKernel[kx, ky];
                result += ptr[offset] * kernalValue;
            }
        }
        return result;
    }
}
