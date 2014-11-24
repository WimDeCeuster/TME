using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IEngineMapper
    {
        Engine MapEngine(Administration.ModelGenerationEngine generationEngine, bool canHaveAssets);
    }
}
