using System.Collections.Generic;
using Avalonia.Controls;
using ICGFilter.Applications;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.ViewModels;
using ICGFilter.Presentation.Views;
using ICGFilter.Presentation.Views.Gamma;

namespace ICGFilter.Domain.Container;

public class WindowContainer
{
    private FileApp _fileApp = new();
    private FilterApp _filterApp = new();
    private PanelApp _panelApp;
    private Dictionary<WindowName, Window> _windows = [];
    public Window GetWindow(WindowName name) => _windows[name]; 
    public WindowContainer()
    {
        var mainWindow = new MainWindow(this);
        _panelApp = new PanelApp(mainWindow.PhotoPanel);
        var viewModel = new MainWindowViewModel(_fileApp, _panelApp, _filterApp); 
        mainWindow.DataContext = viewModel;
        _windows.Add(WindowName.MainWindow, mainWindow);
        CreateGammaWindow();
    }

    private void CreateGammaWindow()
    {
        var gammaWindow = new GammaWindow();
        gammaWindow.DataContext = new GammaSettingsViewModel(_filterApp, _panelApp);
        _windows.Add(WindowName.GammaWindow, gammaWindow);
    }
}