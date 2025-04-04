using System;
using System.Reactive;
using Avalonia.ReactiveUI;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.ViewModels;
using ReactiveUI;

namespace ICGFilter.Presentation.Views.Sobel;

public partial class SobelWindow : ReactiveWindow<SobelSettingsViewModel>
{
    public SobelWindow()
    {
        InitializeComponent();

        DataContextChanged += CloseDialog;
    }

    private void CloseDialog(object? sender, EventArgs e)
    {
        this.WhenActivated(action =>
                action(ViewModel!.CloseInteraction.RegisterHandler(CloseDialogAsync)));
    }

    private void CloseDialogAsync(IInteractionContext<FilterName, Unit> context)
    {
        Close(context.Input);
        context.SetOutput(Unit.Default);
    }
}