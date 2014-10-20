using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ISubModelMapper
    {
        SubModel MapSubModel(ModelGenerationSubModel modelGenerationSubModel);
    }
}