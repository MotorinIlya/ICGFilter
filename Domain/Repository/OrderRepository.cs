namespace ICGFilter.Domain.Repository;

public class OrderlyRepository
{
    private static int[,] _beginKernel = new int[,]
    {
        {0, 2},
        {3, 1}
    };

    public static int[,] GetKernel(int size)
    {
        size = (size > 3) ? 3 : size;
        var tmpKernel = _beginKernel;
        for (var i = 0; i < size; i++)
        {
            var newKernel = new int[tmpKernel.GetLength(0) * 2,tmpKernel.GetLength(0) * 2];
            var sizeBlock = tmpKernel.GetLength(0);
            for (var x = 0; x < sizeBlock; x++)
            {
                for (var y = 0; y < sizeBlock; y++)
                {
                    newKernel[x, y] = 4 * tmpKernel[x, y];
                    newKernel[x + sizeBlock, y] = 4 * tmpKernel[x, y] + 2;
                    newKernel[x, y + sizeBlock] = 4 * tmpKernel[x, y] + 3;
                    newKernel[x + sizeBlock, y + sizeBlock] = 4 * tmpKernel[x, y] + 1;
                }
            }
            tmpKernel = newKernel;
        }
        return tmpKernel;
    }
}