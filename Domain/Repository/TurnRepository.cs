namespace ICGFilter.Domain.Repository;

public class TurnRepository(double min, double max, double val)
{
    private double _min = min;
    private double _max = max;
    private double _value = val;
    public double Min => _min;
    public double Max => _max;
    public double Value
    {
        get => _value;
        set
        {
            if (value <= _max && value >= _min)
            {
                _value = value;
            }
        }
    }
}