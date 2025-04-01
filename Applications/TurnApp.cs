using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Applications;

public class TurnApp
{
    private TurnService _turnService = new();
    private int _turn;
    public WriteableBitmap TurnImage(WriteableBitmap bitmap)
    {
        return _turnService.RotateImage(bitmap, _turn);
    }

    public void SetValueTurn(int turn) => _turn = turn;
}