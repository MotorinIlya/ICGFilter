using System;
using System.Reactive;
using System.Reactive.Linq;
using ICGFilter.Applications;
using ICGFilter.Domain.Filters;
using ICGFilter.Domain.Repository;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public class TurnViewModel : ReactiveObject
{
    private double _turnValue = 1.0;
    private TurnApp _turnApp;
    public double TurnValue
    {
        get => _turnValue; 
        set => this.RaiseAndSetIfChanged(ref _turnValue, value);
    }
    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public Interaction<FilterName, Unit> CloseInteraction = new();

    public TurnViewModel(TurnApp turnApp)
    {
        _turnApp = turnApp;
        
        var canApply = this.WhenAnyValue(
            x => x.TurnValue,
            gamma => gamma >= -180 && gamma <= 180);

        ApplyCommand = ReactiveCommand.CreateFromTask(async () => 
        {
            _turnApp.SetValueTurn((int)_turnValue);
            await CloseInteraction.Handle(FilterName.Turn);
        }, canExecute: canApply);

        CancelCommand = ReactiveCommand.CreateFromTask(async () => 
        { 
            await CloseInteraction.Handle(FilterName.Turn);
        });
    }
}