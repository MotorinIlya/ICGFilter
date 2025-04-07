using System.Collections.Generic;
using ICGFilter.Domain.Filters;

namespace ICGFilter.Domain.Repository;

public class FilterRepository
{
    private readonly Dictionary<FilterName, IImageFilter> _filters = new()
    {
        {FilterName.Inversion, new InversionFilter()},
        {FilterName.BlackWhite, new BlackWhiteFilter()},
        {FilterName.Gamma, new GammaFilter()},
        {FilterName.Sharpness, new SharpnessFitler()},
        {FilterName.Embossing, new EmbossingFitler()},
        {FilterName.Blur, new BlurFitler()},
        {FilterName.Sobel, new SobelFilter()},
        {FilterName.Roberts, new RobertsFilter()},
        {FilterName.Floyd, new FloydSteinbergFilter()},
        {FilterName.WaterColor, new WaterColorFilter()},
    };

    public IImageFilter GetFilter(FilterName filterName) => _filters[filterName];
}