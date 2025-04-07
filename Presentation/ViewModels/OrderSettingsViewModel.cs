using System.Reactive;
using System.Reactive.Linq;
using ICGFilter.Applications;
using ICGFilter.Domain.Filters;
using ICGFilter.Domain.Repository;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public class OrderSettingsViewModel : ReactiveObject
{
    private FilterApp _filterApp;
    private int _rQuant = 3;
    public int RQuant
    {
        get => _rQuant;
        set => this.RaiseAndSetIfChanged(ref _rQuant, value);
    }

    private int _gQuant = 3;
    public int GQuant
    {
        get => _gQuant;
        set => this.RaiseAndSetIfChanged(ref _gQuant, value);
    }

        private int _bQuant = 3;
    public int BQuant
    {
        get => _bQuant;
        set => this.RaiseAndSetIfChanged(ref _bQuant, value);
    }


    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public Interaction<FilterName, Unit> CloseInteraction = new();
    public OrderSettingsViewModel(FilterApp filterApp)
    {
        _filterApp = filterApp;
        var canApply = this.WhenAnyValue(
            x => x.RQuant,
            x => x.GQuant,
            x => x.BQuant,
            (r, g, b) => r >= 2 && r <= 128
                    && g >= 2 && g <= 128
                    && b >= 2 && b <= 128);

        ApplyCommand = ReactiveCommand.CreateFromTask(async () => 
        {
            var filter = _filterApp.GetFilter(FilterName.Orderly);
            if (filter is OrderlyFilter orderFilter)
            {
                orderFilter.RedQuant = _rQuant;
                orderFilter.GreenQuant = _gQuant;
                orderFilter.BlueQuant = _bQuant;
            }
            await CloseInteraction.Handle(FilterName.Orderly);
        }, canExecute: canApply);

        CancelCommand = ReactiveCommand.CreateFromTask(async () => 
        { 
            await CloseInteraction.Handle(FilterName.Orderly);
        });
    }
}