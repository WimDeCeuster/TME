using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarPartMapper
    {
        CarPart MapCarPart(Administration.ModelGenerationCarPart generationCarPart);
    }
}