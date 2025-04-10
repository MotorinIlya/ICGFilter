using System.Collections.Generic;
using ICGFilter.Applications;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.ViewModels;
using ICGFilter.Presentation.Views;
using ReactiveUI;

namespace ICGFilter.Domain.Container;

public class WindowContainer
{
    private FileApp _fileApp = new();
    private FilterApp _filterApp = new();
    private TurnApp _turnApp = new();
    private PanelApp _panelApp;
    private ResizeApp _resizeApp = new();
    private MainWindow _window;
    public MainWindow MainWin => _window;
    private Dictionary<WindowName, ReactiveObject> _models = [];
    public ReactiveObject GetModel(WindowName name) => _models[name]; 
    public WindowContainer()
    {
        _window = new MainWindow(this);
        _panelApp = new PanelApp(_window.PhotoPanel);
        var viewModel = new MainWindowViewModel(_fileApp, _panelApp, 
                _filterApp, _turnApp, _resizeApp); 
        _window.DataContext = viewModel;
        _models.Add(WindowName.MainWindow, viewModel);
        CreateGammaModel();
        CreateBlurModel();
        CreateSobelModel();
        CreateRobertsModel();
        CreateFloydModel();
        CreateOrderModel();
        CreateTurnModel();
    }

    private void CreateGammaModel()
    {
        var model = new GammaSettingsViewModel(_filterApp);
        _models.Add(WindowName.GammaWindow, model);
    }

    private void CreateBlurModel()
    {
        var model = new BlurSettingsViewModel(_filterApp);
        _models.Add(WindowName.BlurWindow, model);
    }

    private void CreateSobelModel()
    {
        var model = new SobelSettingsViewModel(_filterApp);
        _models.Add(WindowName.SobelWindow, model);
    }

    private void CreateRobertsModel()
    {
        var model = new RobertsSettingsViewModel(_filterApp);
        _models.Add(WindowName.RobertsWindow, model);
    }

    private void CreateFloydModel()
    {
        var model = new FloydSettingsViewModel(_filterApp);
        _models.Add(WindowName.FloydWindow, model);
    }

    private void CreateOrderModel()
    {
        var model = new OrderSettingsViewModel(_filterApp);
        _models.Add(WindowName.OrderWindow, model);
    }

    private void CreateTurnModel()
    {
        var model = new TurnViewModel(_turnApp);
        _models.Add(WindowName.TurnWindow, model);
    }
}