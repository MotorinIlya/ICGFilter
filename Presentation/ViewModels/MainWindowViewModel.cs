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
    private FilterName _mode;
    private ResizeName _resizeMode = ResizeName.Bilinear;
    private FilterName _lastMode = FilterName.Default;
    private FileApp _fileApp;
    private PanelApp _panelApp;
    private FilterApp _filterApp;
    private TurnApp _turnApp;
    private ResizeApp _resizeApp;

    public FilterName Mode
    {
        get => _mode;
        set => this.RaiseAndSetIfChanged(ref _mode, value);
    }
    public ResizeName ResizeMode
    {
        get => _resizeMode;
        set => this.RaiseAndSetIfChanged(ref _resizeMode, value);
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
                normalizeResult.Item1, normalizeResult.Item2, ResizeMode);
        _panelApp.SetResizeBitmap(bitmap);
        _panelApp.SetFiltredBitmap(bitmap);
        var turnBitmap = _turnApp.TurnImage(bitmap);
        _panelApp.ChangeBitmap(turnBitmap);
    }

    private async Task ShowDialogFilter(WindowName name)
    {
        Mode = WindowToFilter.GetFilter(name);
        var result = await DialogInteraction.Handle(name);
        SetFilter(result);
    }

    public async Task LoadFile(IStorageFile file)
    {
            var bitmap = await _fileApp.LoadFileAsync(file);
            _panelApp.ChangeAllBitmap(bitmap);
            Mode = FilterName.Original;
            _lastMode = FilterName.Original;
    }

    public async Task SaveFile(IStorageFile file)
    {
        await _fileApp.SaveFileAsync(_panelApp.GetBitmap(), file);
    }

    public void SetFilter(FilterName name)
    {
        if (name == FilterName.Turn)
        {
            var bitmap =_turnApp.TurnImage(_panelApp.GetFiltredBitmap());
            _panelApp.ChangeBitmap(bitmap);
        }
        else if (name != FilterName.Default)
        {
            Mode = name;
            _lastMode = name;
            var newBitmap = _filterApp.ApplyFilter(
                    _panelApp.GetResizeBitmap(), 
                    name);
            _panelApp.SetFiltredBitmap(newBitmap);
            var turnBitmap = _turnApp.TurnImage(newBitmap);
            _panelApp.ChangeBitmap(turnBitmap);
        }
        else
        {
            Mode = _lastMode;
        }
    }

    public void SetOriginal()
    {
        _panelApp.SetResizeBitmap(_panelApp.GetOriginalBitmap());
        _panelApp.SetFiltredBitmap(_panelApp.GetOriginalBitmap());
        var turnBitmap = _turnApp.TurnImage(_panelApp.GetOriginalBitmap());
        _panelApp.ChangeBitmap(turnBitmap);
    }

    public void SetResize(ResizeName name) => ResizeMode = name;
}
