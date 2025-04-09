using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using SkiaSharp;

namespace ICGFilter.Domain.Services;

public class FileSaveService
{
    public static async Task SaveFileAsync(IStorageFile file, WriteableBitmap bitmap)
    {
        using var stream = await file.OpenWriteAsync();
        bitmap.Save(stream);
    }
}