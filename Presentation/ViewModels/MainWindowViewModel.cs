using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ICGFilter.Applications;
using ICGFilter.Domain.Repository;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    private FileApp _fileApp;
    private PanelApp _panelApp;
    private FilterApp _filterApp = new();
    private TurnApp _turnApp = new();
    private double _valTurnSlider = 0;
    public double ValueTurnSlider
    {
        get => _valTurnSlider;
        set
        {
            this.RaiseAndSetIfChanged(ref _valTurnSlider, (int)value);
            var bitmap =_turnApp.TurnImage(_panelApp.GetOriginalBitmap(), (int)value);
            _panelApp.ChangeBitmap(bitmap);
        }
    }

    public MainWindowViewModel(FileApp fileApp, PanelApp panelApp)
    {
        _fileApp = fileApp;
        _panelApp = panelApp;
    }

    public async Task LoadFile(IStorageFile file)
    {
            var bitmap = await _fileApp.LoadFileAsync(file);
            _panelApp.ChangeAllBitmap(bitmap);
    }

    public async Task SaveFile(IStorageFile file)
    {
        await _fileApp.SaveFileAsync(_panelApp.GetBitmap(), file);
    }

    public void SetInverseFilter()
    {
        var invBitmap = _filterApp.GetFilter(
                _panelApp.GetBitmap(), 
                FilterName.Inversion);
        _panelApp.ChangeBitmap(invBitmap);
    }

    public void SetBlackWhiteFilter()
    {
        var bwBitmap = _filterApp.GetFilter(
                _panelApp.GetBitmap(), 
                FilterName.BlackWhite);
        _panelApp.ChangeBitmap(bwBitmap);
    }
}
