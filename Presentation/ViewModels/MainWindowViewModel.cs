using System.Reactive;
using System.Reactive.Linq;
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
    private TurnApp _turnApp;
    private ResizeApp _resizeApp;
    private double _valTurnSlider = 0;
    public double ValueTurnSlider
    {
        get => _valTurnSlider;
        set
        {
            this.RaiseAndSetIfChanged(ref _valTurnSlider, (int)value);
            _turnApp.SetValueTurn((int)_valTurnSlider);
            var bitmap =_turnApp.TurnImage(_panelApp.GetFiltredBitmap());
            _panelApp.ChangeBitmap(bitmap);
        }
    }

    public ReactiveCommand<Unit, Unit> ResizeCommand { get; }
    public ReactiveCommand<WindowName, Unit> OpenShowDialogCommand { get; }
    public Interaction <WindowName, FilterName> DialogInteraction = new();
    public Interaction <Unit, (int, int)> ResizeInteraction = new();

    public MainWindowViewModel(
            FileApp fileApp, 
            PanelApp panelApp, 
            FilterApp filterApp, 
            TurnApp turnApp,
            ResizeApp resizeApp)
    {
        _fileApp = fileApp;
        _panelApp = panelApp;
        _filterApp = filterApp;
        _turnApp = turnApp;
        _resizeApp = resizeApp;

        OpenShowDialogCommand = ReactiveCommand.CreateFromTask<WindowName>(ShowDialogFilter);
        ResizeCommand = ReactiveCommand.CreateFromTask(ResizeImage);
    }

    private async Task ResizeImage()
    {
        var result = await ResizeInteraction.Handle(Unit.Default);
        var normalizeResult = _resizeApp.Normalize(result, _panelApp.GetSizeBitmap());
        var bitmap = _resizeApp.ResizeImage(_panelApp.GetBitmap(), 
                normalizeResult.Item1, normalizeResult.Item2);
        _panelApp.SetResizeBitmap(bitmap);
        _panelApp.SetFiltredBitmap(bitmap);
        var turnBitmap = _turnApp.TurnImage(bitmap);
        _panelApp.ChangeBitmap(turnBitmap);
    }

    private async Task ShowDialogFilter(WindowName name)
    {
        var result = await DialogInteraction.Handle(name);
        SetFilter(result);
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
                _panelApp.GetResizeBitmap(), 
                name);
        _panelApp.SetFiltredBitmap(newBitmap);
        var turnBitmap = _turnApp.TurnImage(newBitmap);
        _panelApp.ChangeBitmap(turnBitmap);
    }

    public void SetOriginal()
    {
        _panelApp.SetFiltredBitmap(_panelApp.GetOriginalBitmap());
        var turnBitmap = _turnApp.TurnImage(_panelApp.GetOriginalBitmap());
        _panelApp.ChangeBitmap(turnBitmap);
    }
}
