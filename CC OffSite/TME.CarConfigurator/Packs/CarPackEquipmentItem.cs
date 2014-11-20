using System;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Equipment;
using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Packs
{
    public class CarPackEquipmentItem<T> : CarEquipmentItem<T>, ICarPackEquipmentItem
        where T: Repository.Objects.Packs.CarPackEquipmentItem
    {
        private Price _price;

        public CarPackEquipmentItem(T repositoryCarPackEquipmentItem, Publication publication, Guid carId, Context context, IAssetFactory assetFactory)
            : base(repositoryCarPackEquipmentItem, publication, carId, context, assetFactory)
        {
            
        }

        public ColouringModes ColouringModes
        {
            get { throw new NotImplementedException(); }
        }

        public override Interfaces.Core.IPrice Price
        {
            get { return _price = _price ?? new Price(RepositoryObject.Price); }
        }
    }
}
