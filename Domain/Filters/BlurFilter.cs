using System;
using Avalonia;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class BlurFitler : IImageFilter
{
    private static float[,] _threeKernel =
    {
        {0, 1, 0},
        {1, 2, 1},
        {0, 1, 0}
    };

    private static float[,] _fiveKernel = 
    {
        {1, 2, 3, 2, 1},
        {2, 3, 4, 3, 2},
        {3, 4, 5, 4, 3},
        {2, 3, 4, 3, 2},
        {1, 2, 3, 2, 1}
    };

    private int _sizeKernel = 3;
    public int SizeKernel
    {
        get => _sizeKernel;
        set
        {
            if (value == 3)
            {
                _sizeKernel = value;
                _actualKernel = _threeKernel;
            }
            else if (value == 5)
            {
                _sizeKernel = value;
                _actualKernel = _fiveKernel;
            }
            else if (value % 2 == 1 && value >= 7 && value <= 11)
            {
                _sizeKernel = value;
                _actualKernel = MatrixService.CreateMatrix( 
                        _sizeKernel, _sizeKernel);
            }
        }
    }

    private float[,] _actualKernel = _threeKernel;

    public BlurFitler()
    {
        MatrixService.MulToKoef(_threeKernel, (float)1/6);
        MatrixService.MulToKoef(_fiveKernel, (float)1/74);
    }

    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        var kernelBorder = (_sizeKernel - 1) / 2;
        using var buf = bitmap.Lock();
        var newBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = newBitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();

        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                var r = 0.0;
                var g = 0.0;
                var b = 0.0;
                for (var kx = -1 * kernelBorder; kx <= kernelBorder; kx++)
                {
                    for (var ky = -1 * kernelBorder; ky <= kernelBorder; ky++)
                    {
                        var px = x + kx;
                        var py = y + ky;
                        if (px < 0 || py < 0 || px >= buf.Size.Width || py >= buf.Size.Height)
                        {
                            continue;
                        }

                        var offset = BitmapService.GetOffset(px, py, newBuf.RowBytes);
                        var kernalValue = _actualKernel[kx + kernelBorder, ky + kernelBorder];

                        r += ptr[offset + 2] * kernalValue;
                        g += ptr[offset + 1] * kernalValue;
                        b += ptr[offset] * kernalValue;
                    }
                }
                
                r = Math.Clamp(r, 0, 255);
                g = Math.Clamp(g, 0, 255);
                b = Math.Clamp(b, 0, 255);

                var origOffset = BitmapService.GetOffset(x, y, newBuf.RowBytes);
                ColorService.SetColor(newPtr, origOffset, (byte)r, (byte)g, (byte)b);
            }
        }
        return newBitmap;
    }
}