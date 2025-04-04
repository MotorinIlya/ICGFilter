using Avalonia.Controls;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.Views.Blur;
using ICGFilter.Presentation.Views.Gamma;
using ICGFilter.Presentation.Views.Sobel;

namespace ICGFilter.Domain.Services;

public class WindowFabric
{
    public static Window CreateWindow(WindowName name)
    {
        if (name == WindowName.GammaWindow)
        {
            return new GammaWindow();
        }
        else if (name == WindowName.BlurWindow)
        {
            return new BlurWindow();
        }
        else if (name == WindowName.SobelWindow)
        {
            return new SobelWindow();
        }
        return null;
    }
}