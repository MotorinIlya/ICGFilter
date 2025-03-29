using Avalonia;
using Avalonia.Media.Imaging;
using System;

namespace ICGFilter.Domain.Services;

public class TurnService
{
    public unsafe WriteableBitmap RotateImage(WriteableBitmap source, double angleDegrees)
    {
        var angleRadians = angleDegrees * Math.PI / 180.0;
        var cos = Math.Cos(angleRadians);
        var sin = Math.Sin(angleRadians);

        var srcWidth = source.PixelSize.Width;
        var srcHeight = source.PixelSize.Height;
        var center = new Point(srcWidth / 2.0, srcHeight / 2.0);

        var (newWidth, newHeight) = CalculateNewSize(srcWidth, srcHeight, angleRadians);

        WriteableBitmap dest = new(
            new PixelSize(newWidth, newHeight),
            source.Dpi,
            source.Format,
            source.AlphaFormat);

        using var srcBuffer = source.Lock();
        using var destBuffer = dest.Lock();
        var srcPtr = (byte*)srcBuffer.Address.ToPointer();
        var destPtr = (byte*)destBuffer.Address.ToPointer();

        var srcBpp = srcBuffer.Format.BitsPerPixel / 8;
        var destBpp = destBuffer.Format.BitsPerPixel / 8;

        var destCenterX = newWidth / 2.0;
        var destCenterY = newHeight / 2.0;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                var xRel = x - destCenterX;
                var yRel = y - destCenterY;

                var srcX = (xRel * cos + yRel * sin) + center.X;
                var srcY = (-xRel * sin + yRel * cos) + center.Y;

                var srcXInt = (int)Math.Round(srcX);
                var srcYInt = (int)Math.Round(srcY);

                if (srcXInt >= 0 && srcXInt < srcWidth && srcYInt >= 0 && srcYInt < srcHeight)
                {
                    var srcIndex = srcYInt * srcBuffer.RowBytes + srcXInt * srcBpp;
                    var destIndex = y * destBuffer.RowBytes + x * destBpp;

                    for (var i = 0; i < srcBpp; i++)
                    {
                        destPtr[destIndex + i] = srcPtr[srcIndex + i];
                    }
                }
                else
                {
                    var destIndex = y * destBuffer.RowBytes + x * destBpp;
                    for (var i = 0; i < destBpp; i++)
                    {
                        destPtr[destIndex + i] = 0;
                    }
                }
            }
        }

        return dest;
    }

    private (int width, int height) CalculateNewSize(int srcWidth, int srcHeight, double angle)
    {
        var cos = Math.Abs(Math.Cos(angle));
        var sin = Math.Abs(Math.Sin(angle));

        var newWidth = (int)(srcWidth * cos + srcHeight * sin);
        var newHeight = (int)(srcWidth * sin + srcHeight * cos);

        return (newWidth, newHeight);
    }
}