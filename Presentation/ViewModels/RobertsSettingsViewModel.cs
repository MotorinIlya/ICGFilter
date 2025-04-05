using System.Reactive;
using System.Reactive.Linq;
using ICGFilter.Applications;
using ICGFilter.Domain.Filters;
using ICGFilter.Domain.Repository;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public class RobertsSettingsViewModel : ReactiveObject
{
    private double _threshold = 1.0;
    private FilterApp _filterApp;
    public double Threshold
    {
        get => _threshold; 
        set => this.RaiseAndSetIfChanged(ref _threshold, (int)value);
    }
    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public Interaction<FilterName, Unit> CloseInteraction = new();

    public RobertsSettingsViewModel(FilterApp filterApp)
    {
        _filterApp = filterApp;
        
        var canApply = this.WhenAnyValue(
            x => x.Threshold,
            threshold => threshold >= 1 && threshold <= 255);

        ApplyCommand = ReactiveCommand.CreateFromTask(async () => 
        {
            var filter = _filterApp.GetFilter(FilterName.Roberts);
            if (filter is RobertsFilter sobelFilter)
            {
                sobelFilter.Threshold = (int)_threshold;
            }
            await CloseInteraction.Handle(FilterName.Roberts);
        }, canExecute: canApply);

        CancelCommand = ReactiveCommand.CreateFromTask(async () => 
        { 
            await CloseInteraction.Handle(FilterName.Roberts);
        });
    }
}