using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using SkiaSharp;

namespace ICGFilter.Domain.Services;

public class FileLoadService
{
    public async static Task<WriteableBitmap?> LoadFile(IStorageFile file) 
    {
        await using var stream = await file.OpenReadAsync();
        return await Task.Run(() =>
        {
            using var skiaStream = new SKManagedStream(stream);
            using var skiaBitmap = SKBitmap.Decode(skiaStream);

            if (skiaBitmap.ColorType != SKColorType.Bgra8888)
            {
                skiaBitmap.CopyTo(skiaBitmap, SKColorType.Bgra8888);
            }

            var writableBitmap = new WriteableBitmap(
                new PixelSize(skiaBitmap.Width, skiaBitmap.Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888
            );

            using var bitmapLock = writableBitmap.Lock();
            unsafe
            {
                var dst = (byte*)bitmapLock.Address.ToPointer();
                var skia = (byte*)skiaBitmap.GetPixels().ToPointer();

                for (var y = 0; y < skiaBitmap.Height; y++)
                {
                    Buffer.MemoryCopy(
                        skia + y * skiaBitmap.RowBytes,
                        dst + y * bitmapLock.RowBytes,
                        skiaBitmap.Width * 4,
                        skiaBitmap.Width * 4
                    );
                }
            }

            return writableBitmap;
        });
    }
}