using System;
using System.Reactive;
using Avalonia.ReactiveUI;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.ViewModels;
using ReactiveUI;

namespace ICGFilter.Presentation.Views.Orderly;

public partial class OrderWindow : ReactiveWindow<OrderSettingsViewModel>
{
    public OrderWindow()
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