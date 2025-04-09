using System;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Repository;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class OrderlyFilter : IImageFilter
{
    private int[,] _redKernel;
    private int[,] _greenKernel;
    private int[,] _blueKernel;
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
        _redKernel = OrderlyRepository.GetKernel(0);
        _greenKernel = OrderlyRepository.GetKernel(0);
        _blueKernel = OrderlyRepository.GetKernel(0);
    }

    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        using var buf = bitmap.Lock();
        (var redSize, var greenSize, var blueSize) = FindMaxSizeForKernel(
                buf.Size.Width, buf.Size.Height);
        _redKernel = OrderlyRepository.GetKernel(redSize);
        _greenKernel = OrderlyRepository.GetKernel(greenSize);
        _blueKernel = OrderlyRepository.GetKernel(blueSize);
        var newBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = newBitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();
        
        redSize = _redKernel.GetLength(0);
        greenSize = _greenKernel.GetLength(0);
        blueSize = _blueKernel.GetLength(0);

        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                (var r, var g, var b) = ColorService.GetColor(
                        ptr, BitmapService.GetOffset(x, y, buf.RowBytes));

                var newR = r + 256
                        * ((float)_redKernel[x % redSize, y % redSize] 
                        / (redSize * redSize)  - 0.5);
                var newG = g + 256
                        * ((float)_greenKernel[x % greenSize, y % greenSize] 
                        / (greenSize * greenSize)  - 0.5);
                var newB = b + 256
                        * ((float)_blueKernel[x % blueSize, y % blueSize] 
                        / (blueSize * blueSize)  - 0.5);
                (var quantR, var quantG, var quantB) = QuantPixel(
                        (float)newR, (float)newG, (float)newB);
                
                var offset = BitmapService.GetOffset(x, y, buf.RowBytes);
                ColorService.SetColor(newPtr, offset,
                    (byte)quantR, (byte)quantG, (byte)quantB);
            }
        }
        return newBitmap;
    }

    private (int, int, int) FindMaxSizeForKernel(int width, int height)
    {
        var redSize = 256 / _redQuant;
        var greenSize = 256 / _greenQuant;
        var blueSize = 256 / _blueQuant;
        
        var red = 4;
        while (redSize > red)
        {
            red *= 4;
        }
        red /= 4;

        var green = 4;
        while (greenSize > green)
        {
            green *= 4;
        }
        green /= 4;

        var blue = 4;
        while (blueSize > blue)
        {
            blue *= 4;
        }
        blue /= 4;
        return (red, green, blue);
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