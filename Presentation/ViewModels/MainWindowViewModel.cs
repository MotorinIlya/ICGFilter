using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ICGFilter.Applications;
using ICGFilter.Domain.Enums;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    private FileApp _fileApp;
    private PanelApp _panelApp;

    public MainWindowViewModel(FileApp fileApp, PanelApp panelApp)
    {
        _fileApp = fileApp;
        _panelApp = panelApp;
    }

    public async Task LoadFile(IStorageFile file)
    {
            var bitmap = await _fileApp.LoadFileAsync(file);
            _panelApp.ChangeBitmap(bitmap);
    }

    public async Task SaveFile(IStorageFile file)
    {
        await _fileApp.SaveFileAsync(_panelApp.GetBitmap(), file);
    }
}
