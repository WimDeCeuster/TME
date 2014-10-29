using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ISubModelMapper
    {
        SubModel MapSubModel(ModelGenerationGrade[] generationSubModel, ModelGenerationSubModel modelGenerationSubModel, ContextData contextData, bool isPreview);
    }
}