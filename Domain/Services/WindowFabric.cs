using Avalonia.Controls;
using ICGFilter.Domain.Repository;
using ICGFilter.Presentation.Views.About;
using ICGFilter.Presentation.Views.Blur;
using ICGFilter.Presentation.Views.FloydSteinberg;
using ICGFilter.Presentation.Views.Gamma;
using ICGFilter.Presentation.Views.Orderly;
using ICGFilter.Presentation.Views.Roberts;
using ICGFilter.Presentation.Views.Sobel;
using ICGFilter.Presentation.Views.Turn;

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
        else if (name == WindowName.RobertsWindow)
        {
            return new RobertsWindow();
        }
        else if (name == WindowName.FloydWindow)
        {
            return new FloydWindow();
        }
        else if (name == WindowName.OrderWindow)
        {
            return new OrderWindow();
        }
        else if (name == WindowName.TurnWindow)
        {
            return new TurnWindow();
        }
        else if (name == WindowName.AboutWindow)
        {
            return new AboutWindow();
        }
        return null;
    }
}