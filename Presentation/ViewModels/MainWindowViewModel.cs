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
    private FilterApp _filterApp;
    private TurnApp _turnApp = new();
    private double _valTurnSlider = 0;
    public double ValueTurnSlider
    {
        get => _valTurnSlider;
        set
        {
            this.RaiseAndSetIfChanged(ref _valTurnSlider, (int)value);
            var bitmap =_turnApp.TurnImage(_panelApp.GetFiltredBitmap(), (int)value);
            _panelApp.ChangeBitmap(bitmap);
        }
    }

    public MainWindowViewModel(FileApp fileApp, PanelApp panelApp, FilterApp filterApp)
    {
        _fileApp = fileApp;
        _panelApp = panelApp;
        _filterApp = filterApp;
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

    public void SetFilter(FilterName name)
    {
        var newBitmap = _filterApp.ApplyFilter(
                _panelApp.GetOriginalBitmap(), 
                name);
        _panelApp.SetFiltredBitmap(newBitmap);
        var turnBitmap = _turnApp.TurnImage(newBitmap, (int)_valTurnSlider);
        _panelApp.ChangeBitmap(turnBitmap);
    }
}
