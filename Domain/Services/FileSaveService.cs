using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using SkiaSharp;

namespace ICGFilter.Domain.Services;

public class FileSaveService
{
    public async static Task SaveImageAsync(IStorageFile file, WriteableBitmap bitmap)
    {
        using var skImage = ConvertToSkiaImage(bitmap);
        if (skImage == null) 
        {
            return;
        }

        await using var stream = await file.OpenWriteAsync();
        using var data = skImage.Encode(GetFormat(file.Name), 90);
        data.SaveTo(stream);
    }

    private static SKImage ConvertToSkiaImage(WriteableBitmap bitmap)
    {
        using var locked = bitmap.Lock();
        var info = new SKImageInfo(
            locked.Size.Width,
            locked.Size.Height,
            SKColorType.Bgra8888,
            SKAlphaType.Premul);

        return SKImage.FromPixelCopy(info, locked.Address);
    }

    private static SKEncodedImageFormat GetFormat(string fileName)
    {
        return Path.GetExtension(fileName).ToLower() switch
        {
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
            _ => SKEncodedImageFormat.Png
        };
    }
}