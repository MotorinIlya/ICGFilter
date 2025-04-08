using System.Reactive;
using System.Reactive.Linq;
using ICGFilter.Applications;
using ICGFilter.Domain.Filters;
using ICGFilter.Domain.Repository;
using ReactiveUI;

namespace ICGFilter.Presentation.ViewModels;

public class BlurSettingsViewModel : ReactiveObject
{
    private FilterApp _filterApp;
    private int _sizeKernel = 3;
    public int SizeKernel
    {
        get => _sizeKernel;
        set => this.RaiseAndSetIfChanged(ref _sizeKernel, value);
    }

    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public Interaction<FilterName, Unit> CloseInteraction = new();
    public BlurSettingsViewModel(FilterApp filterApp)
    {
        _filterApp = filterApp;
        var canApply = this.WhenAnyValue(
            x => x.SizeKernel,
            size => size >= 3 && size <= 11 && size % 2 == 1);

        ApplyCommand = ReactiveCommand.CreateFromTask(async () => 
        {
            var filter = _filterApp.GetFilter(FilterName.Blur);
            if (filter is BlurFitler gammaFilter)
            {
                gammaFilter.SizeKernel = _sizeKernel;
            }
            await CloseInteraction.Handle(FilterName.Blur);
        }, canExecute: canApply);

        CancelCommand = ReactiveCommand.CreateFromTask(async () => 
        { 
            await CloseInteraction.Handle(FilterName.Default);
        });
    }
}