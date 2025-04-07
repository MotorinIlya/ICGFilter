using System;
using System.Collections.Generic;
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

    // private unsafe (int, int, int) CountNewPixel(byte* ptr, int x, int y, int rowBytes,
    //         float[] rmatrix, float[] gmatrix, float[] bmatrix, int size)
    // {
    //     (var r, var g, var b) = ColorService.GetColor(
    //             ptr, BitmapService.GetOffset(x, y, rowBytes));
    //     var newR = r + 256 / (size * size) 
    //             * (_kernel[x % _kernel.GetLength(0), y % _kernel.GetLength(0)];
    // }
}