using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ICGFilter.Applications;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    private FileApp _fileApp;
    private PanelApp _panelApp;
    private FilterApp _filterApp;
    public ReactiveCommand<Unit, Unit> InverseCommand { get; }

    public MainWindowViewModel(
            FileApp fileApp, 
            PanelApp panelApp, 
            FilterApp filterApp)
    {
        _fileApp = fileApp;
        _panelApp = panelApp;
        _filterApp = filterApp;

        InverseCommand = ReactiveCommand.Create(SetInverseFilter);
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

    public void SetInverseFilter()
    {
        var invBitmap = _filterApp.SetInverseFilter(_panelApp.GetBitmap());
        _panelApp.ChangeBitmap(invBitmap);
    }
}
