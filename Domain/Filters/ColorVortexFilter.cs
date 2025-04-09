using Avalonia.Media.Imaging;

namespace ICGFilter.Domain.Filters;

using Avalonia;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;
using System;
using System.Runtime.InteropServices;

public class ColorVortexFilter : IImageFilter
{
    private static readonly float[,] KernelR = 
    {
        { 0f, 0.4f, 0f },
        { 0f, 0f,  0f },
        { 0f, 0.4f, 0f }
    };

    private static readonly float[,] KernelG = 
    {
        { 0f,  0f,  0f },
        { 0.4f, 0f, 0.4f },
        { 0f,  0f,  0f }
    };

    private static readonly float[,] KernelB = 
    {
        { 0.3f, 0f, 0.3f },
        { 0f,  0f,  0f },
        { 0.3f, 0f, 0.3f }
    };

    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {

        using var buf = bitmap.Lock();
        var resultBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = resultBitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();

        ProcessChannels(ptr, newPtr, buf.Size.Width, buf.Size.Height, buf.RowBytes);

        return resultBitmap;
    }

    private unsafe void ProcessChannels(byte* ptr, byte* newPtr,
             int width, int height, int stride)
    {     
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var offset = BitmapService.GetOffset(x, y, stride);
                var (r, g, b) = ApplyKernels(ptr, offset, x, y, stride, width, height);
                
                newPtr[offset]     = (byte)Math.Clamp(b, 0, 255);
                newPtr[offset + 1] = (byte)Math.Clamp(g, 0, 255);
                newPtr[offset + 2] = (byte)Math.Clamp(r, 0, 255);
            }
        }
    }

    private unsafe (float r, float g, float b) ApplyKernels(byte* pixels, int offset,
            int x, int y, int rowBytes, int width, int height)
    {
        float originalR = pixels[offset + 2];
        float originalG = pixels[offset + 1];
        float originalB = pixels[offset];
        
        float r = originalR + Convolve(pixels, x, y, width, height, rowBytes, KernelR, c => c.G);
        float g = originalG + Convolve(pixels, x, y, width, height, rowBytes, KernelG, c => c.B);
        float b = originalB + Convolve(pixels, x, y, width, height, rowBytes, KernelB, c => c.R);
        
        return (r, g, b);
    }

    private unsafe float Convolve(
        byte* pixels,
        int x,
        int y,
        int width,
        int height,
        int rowBytes,
        float[,] kernel, 
        Func<(byte R, byte G, byte B), float> channelSelector)
    {
        float sum = 0;
        
        for (int ky = -1; ky <= 1; ky++)
        {
            for (int kx = -1; kx <= 1; kx++)
            {
                var px = x + kx;
                var py = y + ky;
                if (px < 0 || py < 0 || px >= width || py >= height)
                {
                    continue;
                }
                var offset = BitmapService.GetOffset(px, py, rowBytes);
                var pixel = (
                    R: pixels[offset + 2],
                    G: pixels[offset + 1],
                    B: pixels[offset]
                );
                
                var weight = kernel[ky + 1, kx + 1];
                sum += channelSelector(pixel) * weight;
            }
        }
        
        return sum;
    }
}