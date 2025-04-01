using System.Collections.Generic;
using Avalonia.Controls;
using ICGFilter.Applications;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.ViewModels;
using ICGFilter.Presentation.Views;
using ICGFilter.Presentation.Views.Gamma;
using ReactiveUI;

namespace ICGFilter.Domain.Container;

public class WindowContainer
{
    private FileApp _fileApp = new();
    private FilterApp _filterApp = new();
    private TurnApp _turnApp = new();
    private PanelApp _panelApp;
    private MainWindow _window;
    public MainWindow MainWin => _window;
    private Dictionary<WindowName, ReactiveObject> _models = [];
    public ReactiveObject GetModel(WindowName name) => _models[name]; 
    public WindowContainer()
    {
        _window = new MainWindow(this);
        _panelApp = new PanelApp(_window.PhotoPanel);
        var viewModel = new MainWindowViewModel(_fileApp, _panelApp, _filterApp, _turnApp); 
        _window.DataContext = viewModel;
        _models.Add(WindowName.MainWindow, viewModel);
        //_windows.Add(WindowName.MainWindow, mainWindow);
        CreateGammaModel();
    }

    private void CreateGammaModel()
    {
        var model = new GammaSettingsViewModel(_filterApp, _panelApp, _turnApp);
        _models.Add(WindowName.GammaWindow, model);
    }
}