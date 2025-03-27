using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using ICGFilter.Domain.Services;

namespace ICGFilter.Applications;

public class FileApp(Window window)
{
    Window _window = window;
    public async Task<WriteableBitmap?> LoadFileAsync(IStorageFile file)
    {
        return await FileLoadService.LoadFile(file);
    }

    public async Task SaveFileAsync(WriteableBitmap bitmap, IStorageFile file)
    {
        await FileSaveService.SaveImageAsync(file, bitmap);
    }
}