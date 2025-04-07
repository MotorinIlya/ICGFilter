using System;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Repository;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class OrderlyFilter : IImageFilter
{
    private int[,] _kernel;
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
    public OrderlyFilter()
    {
        _kernel = OrderlyRepository.GetKernel(0);
    }

    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        using var buf = bitmap.Lock();
        _kernel = OrderlyRepository.GetKernel(
                FindMaxSizeForKernel(buf.Size.Width, buf.Size.Height));
        var newBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = newBitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();
        
        var size = _kernel.GetLength(0);
        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                (var r, var g, var b) = ColorService.GetColor(
                        ptr, BitmapService.GetOffset(x, y, buf.RowBytes));

                var newR = r + 256
                        * ((float)_kernel[x % size, y % size] / (size * size)  - 0.5);
                var newG = g + 256
                        * ((float)_kernel[x % size, y % size] / (size * size)  - 0.5);
                var newB = b + 256
                        * ((float)_kernel[x % size, y % size] / (size * size)  - 0.5);
                (var quantR, var quantG, var quantB) = QuantPixel(
                        (float)newR, (float)newG, (float)newB);
                
                var offset = BitmapService.GetOffset(x, y, buf.RowBytes);
                ColorService.SetColor(newPtr, offset,
                    (byte)quantR, (byte)quantG, (byte)quantB);
            }
        }
        return newBitmap;
    }

    private int FindMaxSizeForKernel(int width, int height)
    {
        var size = 0;
        width /= 2;
        height /= 2;
        while (width % 2 == 0 && height % 2 == 0)
        {
            width /= 2;
            height /= 2;
            size++;
        }
        return size;
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
}