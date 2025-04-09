using System;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Repository;
using ICGFilter.Domain.Services;

namespace ICGFilter.Applications;

public class ResizeApp
{
    public WriteableBitmap ResizeImage(WriteableBitmap bitmap, 
            int width, int height, ResizeName resizeName)
    {
        if (resizeName == ResizeName.Bilinear)
        {
            return ImageResizeService.BilinearResize(bitmap, width, height);
        }
        else 
        {
            return ImageResizeService.NearestNeighborResize(bitmap, width, height);
        }
    }

    public (int, int) Normalize((int, int) newSize, (int, int) oldSize)
    {
        var x = (float)newSize.Item1 / oldSize.Item1;
        var y = (float)newSize.Item2 / oldSize.Item2;
        var res = Math.Min(x, y);
        var tmpx = (int)(oldSize.Item1 * res);
        var tmpy = (int)(oldSize.Item2 * res);
        return (tmpx, tmpy);
    }
}