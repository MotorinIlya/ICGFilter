using Avalonia;
using Avalonia.Controls;

namespace ICGFilter.Domain.Repository;

public class ScrollRepository(Point point, ScrollViewer scrollViewer)
{
    public bool IsPanning = false;
    public Point LastPressedPoint = point;
    public ScrollViewer ScrollViewer = scrollViewer;
}