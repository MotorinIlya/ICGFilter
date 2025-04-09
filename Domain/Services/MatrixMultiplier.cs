using System;

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

    public static float[,] CreateMatrix(int width, int height)
    {
        var matrix = new float[width, height];
        var mean = width / 2;
        var sum = 0f;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                matrix[x, y] = (float)Math.Exp(-(Math.Pow(x - mean, 2) + Math.Pow(y - mean, 2)) / (2));
                sum += matrix[x, y];
            }
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                matrix[x, y] /= sum;
            }
        }
        return matrix;
    }
}