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
    }
}