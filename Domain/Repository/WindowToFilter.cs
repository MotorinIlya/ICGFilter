using System.Collections.Generic;

namespace ICGFilter.Domain.Repository;

public class WindowToFilter
{
    private static readonly Dictionary<WindowName, FilterName> _filters = new()
    {
        {WindowName.BlurWindow, FilterName.Blur},
        {WindowName.FloydWindow, FilterName.Floyd},
        {WindowName.GammaWindow, FilterName.Gamma},
        {WindowName.OrderWindow, FilterName.Orderly},
        {WindowName.RobertsWindow, FilterName.Roberts},
        {WindowName.SobelWindow, FilterName.Sobel},
        {WindowName.TurnWindow, FilterName.Turn}
    };

    public static FilterName GetFilter(WindowName name) => _filters[name];
}