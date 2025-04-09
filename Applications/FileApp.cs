using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using ICGFilter.Domain.Services;

namespace ICGFilter.Applications;

public class FileApp()
{
    public async Task<WriteableBitmap?> LoadFileAsync(IStorageFile file)
    {
        return await FileLoadService.LoadFile(file);
    }

    public async Task SaveFileAsync(WriteableBitmap bitmap, IStorageFile file)
    {
        await FileSaveService.SaveFileAsync(file, bitmap);
    }
}