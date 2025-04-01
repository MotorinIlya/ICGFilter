using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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

    public ReactiveCommand<WindowName, Unit> OpenShowDialogCommand { get; }
    public Interaction <WindowName, Unit> DialogInteraction = new();

    public MainWindowViewModel(
            FileApp fileApp, 
            PanelApp panelApp, 
            FilterApp filterApp, 
            TurnApp turnApp)
    {
        _fileApp = fileApp;
        _panelApp = panelApp;
        _filterApp = filterApp;
        _turnApp = turnApp;

        OpenShowDialogCommand = ReactiveCommand.CreateFromTask<WindowName>(ShowDialogFilter);
    }

    private async Task ShowDialogFilter(WindowName name)
    {
        await DialogInteraction.Handle(name);
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
        var turnBitmap = _turnApp.TurnImage(newBitmap);
        _panelApp.ChangeBitmap(turnBitmap);
    }
}
