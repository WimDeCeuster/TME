using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IBodyTypeMapper
    {
        BodyType MapBodyType(Administration.ModelGenerationBodyType generationBodyType, bool canHaveAssets);
    }
}
