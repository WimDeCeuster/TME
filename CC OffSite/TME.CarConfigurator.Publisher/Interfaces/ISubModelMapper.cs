using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects;
using Model = TME.CarConfigurator.Administration.Model;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ISubModelMapper
    {
        SubModel MapSubModel(Model model, ModelGenerationSubModel modelGenerationSubModel, TimeFrame timeFrame, bool isPreview);
    }
}