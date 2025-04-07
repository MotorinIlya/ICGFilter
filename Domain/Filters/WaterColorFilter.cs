using System.Collections.Generic;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class WaterColorFilter : IImageFilter
{
    private SharpnessFitler _filter = new();
    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        using var buf = bitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();

        var newBitmap = ApplyMedianFilter(
                ptr, buf.Size.Width, buf.Size.Height, buf.RowBytes);
        return _filter.Apply(newBitmap);
    }

    private unsafe WriteableBitmap ApplyMedianFilter(byte* ptr,
            int width, int height, int rowBytes)
    {
        var newBitmap = BitmapService.CreateBitmap(width, height);
        using var buf = newBitmap.Lock();
        var newPtr = (byte*)buf.Address.ToPointer();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var rarray = new List<int>();
                var garray = new List<int>();
                var barray = new List<int>();

                for (var i = -2; i <= 2; i++)
                {
                    for (var j = -2; j <= 2; j++)
                    {
                        var px = x + i;
                        var py = y + j;
                        if (px < 0 || px >= width || py < 0 || py >= height)
                        {
                            continue;
                        }
                        var offset = BitmapService.GetOffset(px, py, rowBytes);
                        (var r, var g, var b) = ColorService.GetColor(ptr, offset);
                        rarray.Add(r);
                        garray.Add(g);
                        barray.Add(b);
                    }
                }
                rarray.Sort();
                garray.Sort();
                barray.Sort();

                var origOffset = BitmapService.GetOffset(x, y, rowBytes);
                ColorService.SetColor(newPtr, origOffset, 
                    (byte)rarray[rarray.Count / 2 + 1], 
                    (byte)garray[garray.Count / 2 + 1],
                    (byte)barray[barray.Count / 2 + 1]);
            }
        }
        return newBitmap;
    }
}
