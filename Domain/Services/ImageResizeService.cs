using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Skia;

namespace ICGFilter.Domain.Services;

public class ImageResizeService
{
    public static WriteableBitmap ResizeWithSkia(
            WriteableBitmap bitmap, 
            int newWidth, int newHeight)
    {
        
        
        return bitmap;
    }
    public unsafe static WriteableBitmap BilinearResize(
            WriteableBitmap source, 
            int newWidth, int newHeight)
    {
        var dest = BitmapService.CreateBitmap(newWidth, newHeight);

        using var srcBuffer = source.Lock();
        using var dstBuffer = dest.Lock();

        var ptr = (byte*)srcBuffer.Address.ToPointer();
        var newPtr = (byte*)dstBuffer.Address.ToPointer();

        float xRatio = (float)(srcBuffer.Size.Width - 1) / newWidth;
        float yRatio = (float)(srcBuffer.Size.Height - 1) / newHeight;

        for (int y = 0; y < newHeight; y++)
        {
            float srcY = y * yRatio;
            int yFloor = (int)Math.Floor(srcY);
            float yFraction = srcY - yFloor;

            for (int x = 0; x < newWidth; x++)
            {
                float srcX = x * xRatio;
                int xFloor = (int)Math.Floor(srcX);
                float xFraction = srcX - xFloor;

                // Получаем 4 соседних пикселя
                var p1 = GetPixel(
                    ptr, srcBuffer.Size.Width, srcBuffer.Size.Height, 
                    xFloor, yFloor, srcBuffer.RowBytes);
                var p2 = GetPixel(
                    ptr, srcBuffer.Size.Width, srcBuffer.Size.Height, 
                    xFloor + 1, yFloor, srcBuffer.RowBytes);
                var p3 = GetPixel(
                    ptr, srcBuffer.Size.Width, srcBuffer.Size.Height, 
                    xFloor, yFloor + 1, srcBuffer.RowBytes);
                var p4 = GetPixel(
                    ptr, srcBuffer.Size.Width, srcBuffer.Size.Height, 
                    xFloor + 1, yFloor + 1, srcBuffer.RowBytes);

                // Интерполяция по горизонтали
                var top = Lerp(p1, p2, xFraction);
                var bottom = Lerp(p3, p4, xFraction);
                
                // Интерполяция по вертикали
                var result = Lerp(top, bottom, yFraction);

                var offset = BitmapService.GetOffset(x, y, dstBuffer.RowBytes);
                ColorService.SetColor(
                        newPtr, offset, result.Item1, result.Item2, result.Item3);
            }
        }

        return dest;
    }

    private unsafe static (int, int, int) GetPixel(byte* pixels, 
            int width, int height, int x, int y, int rowBytes)
    {
        x = Math.Clamp(x, 0, width - 1);
        y = Math.Clamp(y, 0, height - 1);
        var offset = BitmapService.GetOffset(x, y, rowBytes);
        return (pixels[offset + 2], pixels[offset + 1], pixels[offset]);
    }

    private unsafe static (byte, byte, byte) Lerp(
            (int, int, int) a, (int, int, int) b, float t)
    {
        return (
            (byte)(a.Item1 + (b.Item1 - a.Item1) * t),
            (byte)(a.Item2 + (b.Item2 - a.Item2) * t),
            (byte)(a.Item3 + (b.Item3 - a.Item3) * t)
        );
    }
}