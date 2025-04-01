using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ICGFilter.Presentation.ViewModels;
using ReactiveUI;

namespace ICGFilter.Presentation.Views.Gamma;

public partial class GammaWindow : ReactiveWindow<GammaSettingsViewModel>
{
    public GammaWindow()
    {
        InitializeComponent();
        
        DataContextChanged += CloseDialog;
    }

    private void CloseDialog(object? sender, EventArgs e)
    {
        this.WhenActivated(action =>
                action(ViewModel!.CloseInteraction.RegisterHandler(CloseDialogAsync)));
    }

    private void CloseDialogAsync(IInteractionContext<Unit, Unit> context)
    {
        Close();
        context.SetOutput(Unit.Default);
    }
}