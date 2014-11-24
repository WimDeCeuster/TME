using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPackMapper
    {
        GradePack MapGradePack(ModelGenerationGradePack gradePack, ModelGenerationPack generationPack, IReadOnlyCollection<Car> gradeCars);
        Repository.Objects.Packs.CarPack MapCarPack(Administration.CarPack carPack, EquipmentGroups groups, EquipmentCategories categories, bool isPreview, string assetUrl);
    }
}