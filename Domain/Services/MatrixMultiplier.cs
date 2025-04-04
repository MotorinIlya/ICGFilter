namespace ICGFilter.Domain.Services;

public static class MatrixService
{
    public static void MulToKoef(float [,] matrix, float koef)
    {
        var width = matrix.GetLength(0);
        var height = matrix.GetLength(1);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                matrix[x, y] *= koef;
            }
        }
    }

    public static float[,] CreateMatrix(float num, int width, int height)
    {
        var matrix = new float[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                matrix[x, y] = num;
            }
        }
        return matrix;
    }
}