using Avalonia.Controls;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.Views.Gamma;

namespace ICGFilter.Domain.Services;

public class WindowFabric
{
    public static Window CreateWindow(WindowName name)
    {
        if (name == WindowName.GammaWindow)
        {
            return new GammaWindow();
        }
        return null;
    }
}