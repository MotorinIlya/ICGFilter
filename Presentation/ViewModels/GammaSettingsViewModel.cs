using System;
using System.Reactive;
using System.Reactive.Linq;
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
    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public Interaction<Unit, Unit> CloseInteraction = new();

    public GammaSettingsViewModel(FilterApp filterApp, PanelApp panelApp, TurnApp turnApp)
    {
        _filterApp = filterApp;
        _panelApp = panelApp;
        _turnApp = turnApp;
        var canApply = this.WhenAnyValue(
            x => x.GammaValue,
            gamma => gamma >= 0.1 && gamma <= 5.0);

        ApplyCommand = ReactiveCommand.CreateFromTask(async () => 
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
            await CloseInteraction.Handle(Unit.Default);
        }, canExecute: canApply);

        CancelCommand = ReactiveCommand.CreateFromTask(async () => 
        { 
            await CloseInteraction.Handle(Unit.Default);
        });
    }
}