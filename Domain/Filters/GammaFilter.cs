using System;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class GammaFilter : IImageFilter
{
    private double _gamma = 1.0;
    public double Gamma
    {
        get => _gamma;
        set
        {
            _gamma = (value > 0) ? value : _gamma;
        }
    }

    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        using var buf = bitmap.Lock();
        var newBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = newBitmap.Lock();

        var gammaTable = new byte[256];
        for (var i = 0; i < 256; i++)
        {
            var tmp = Math.Pow((double)i / 255, _gamma) * 255;
            gammaTable[i] = (byte)tmp;
        }

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();

        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                var offset = BitmapService.GetOffset(x, y, buf.RowBytes);
                (var r, var g, var b) = ColorService.GetColor(ptr, offset);
                ColorService.SetColor(
                        newPtr, offset, 
                        gammaTable[r], 
                        gammaTable[g], 
                        gammaTable[b]);
            }
        }
        return newBitmap;
    }
}