using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class FloydSteinbergFilter : IImageFilter
{
    private int _redQuant = 3;
    private int _greenQuant = 3;
    private int _blueQuant = 3;
    public int RedQuant
    {
        get => _redQuant;
        set 
        {
            if (value >= 2 && value <= 128)
            {
                _redQuant = value;
            }
        }
    }

    public int GreenQuant
    {
        get => _greenQuant;
        set 
        {
            if (value >= 2 && value <= 128)
            {
                _greenQuant = value;
            }
        }
    }

    public int BlueQuant
    {
        get => _blueQuant;
        set 
        {
            if (value >= 2 && value <= 128)
            {
                _blueQuant = value;
            }
        }
    }
    private static readonly float[,] _kernel =
    {
        {0, 0, 7.0f/16},
        {3.0f/16, 5.0f/16, 1.0f/16}
    };
    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        using var buf = bitmap.Lock();
        var newBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = newBitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();
        (var rmatrix, var gmatrix, var bmatrix) = BitmapService.ToMatrix(buf);

        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                var offset = BitmapService.GetOffset(x, y, buf.RowBytes);
                (var r, var g, var b) = QuantPixel(rmatrix[x,y], gmatrix[x,y], bmatrix[x,y]);
                ColorService.SetColor(newPtr, offset, (byte)r, (byte)g, (byte)b);
                AddError(rmatrix, gmatrix, bmatrix, r, g, b, x, y);
            }
        }
        return newBitmap;
    }

    private (int, int, int) QuantPixel(float r, float g, float b)
    {
        var tmpb = Math.Clamp((double)r, 0, 255);
        tmpb = Math.Round(tmpb / ((double)256/(_blueQuant - 1)));
        tmpb *= 256/(_blueQuant - 1);
        tmpb = Math.Clamp(tmpb, 0, 255);

        var tmpg = Math.Clamp((double)g, 0, 255);
        tmpg = Math.Round(tmpg / ((double)256/(_greenQuant - 1)));
        tmpg *= 256/(_greenQuant - 1);
        tmpg = Math.Clamp(tmpg, 0, 255);

        var tmpr = Math.Clamp((double)b, 0, 255);
        tmpr = Math.Round(tmpr / ((double)256/(_redQuant - 1)));
        tmpr *= 256/(_redQuant - 1);
        tmpr = Math.Clamp(tmpr, 0, 255);

        return ((int)tmpb, (int)tmpg, (int)tmpr);
    }

    private void AddError(
            float[,] rmatrix, float[,] gmatrix, float[,] bmatrix, 
            int r, int g, int b,
            int x, int y)
    {
            var errorb = bmatrix[x, y] - b;
            var errorg = gmatrix[x, y] - g;
            var errorr = rmatrix[x, y] - r;
            
            var width = rmatrix.GetLength(0);
            var height = rmatrix.GetLength(1);
            for (var i = -1; i <= 1; i++)
            {
                for (var j = 0; j <= 1; j++)
                {
                    var px = x + i;
                    var py = y + j;
                    if (px < 0 || px >= width || py < 0 || py >= height)
                    {
                        continue;
                    }

                    rmatrix[px, py] += errorr * _kernel[j, i + 1];
                    gmatrix[px, py] += errorg * _kernel[j, i + 1];
                    bmatrix[px, py] += errorb * _kernel[j, i + 1];
                }
            }
    }
}
