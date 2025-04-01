using System;
using System.Reactive;
using ICGFilter.Applications;
using ICGFilter.Domain.Filters;
using ICGFilter.Domain.Repository;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public class GammaSettingsViewModel : ReactiveObject
{
    private double _gammaValue = 1.0;
    private FilterApp _filterApp;
    private PanelApp _panelApp;
    private TurnApp _turnApp;
    public double GammaValue
    {
        get => _gammaValue; 
        set => this.RaiseAndSetIfChanged(ref _gammaValue, value);
    }
    private bool _isVisible = false;
    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }
    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public GammaSettingsViewModel(FilterApp filterApp, PanelApp panelApp, TurnApp turnApp)
    {
        _filterApp = filterApp;
        _panelApp = panelApp;
        _turnApp = turnApp;
        var canApply = this.WhenAnyValue(
            x => x.GammaValue,
            gamma => gamma >= 0.1 && gamma <= 5.0);

        ApplyCommand = ReactiveCommand.Create(() => 
        {
            var filter = _filterApp.GetFilter(FilterName.Gamma);
            if (filter is GammaFilter gammaFilter)
            {
                gammaFilter.Gamma = _gammaValue;
            }
            var bitmap = _filterApp.ApplyFilter(
                        _panelApp.GetOriginalBitmap(), 
                        FilterName.Gamma);
            _panelApp.SetFiltredBitmap(bitmap);
            _panelApp.ChangeBitmap(bitmap);
            var turnBitmap = _turnApp.TurnImage(bitmap);
            IsVisible = false;
        }, canExecute: canApply);

        CancelCommand = ReactiveCommand.Create(() => 
        { 
            IsVisible = false;
        });
    }
}