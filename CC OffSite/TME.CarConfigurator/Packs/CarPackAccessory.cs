using System;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Packs
{
    public class CarPackAccessory : CarPackEquipmentItem<Repository.Objects.Packs.CarPackAccessory>, ICarPackAccessory
    {
        public CarPackAccessory(Repository.Objects.Packs.CarPackAccessory repositoryCarPackAccessory, Publication publication, Guid carId, Context context, IAssetFactory assetFactory)
            : base(repositoryCarPackAccessory, publication, carId, context, assetFactory)
        {

        }
    }
}
