namespace ICGFilter.Domain.Services;

public class ColorService
{
    public static unsafe void SetColor(byte* ptr, int offset, byte r, byte g, byte b)
    {
            ptr[offset] = b;
            ptr[offset + 1] = g;
            ptr[offset + 2] = r;
    }

    /// <summary>
    /// возвращает три byte числа в порядке RGB
    /// </summary>
    public static unsafe (byte, byte, byte) GetColor(byte* ptr, int offset)
    {
        return (ptr[offset + 2], ptr[offset + 1], ptr[offset]);
    }
}