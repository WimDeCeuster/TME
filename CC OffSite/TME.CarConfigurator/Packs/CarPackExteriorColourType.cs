using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Packs
{
    public class CarPackExteriorColourType : CarPackEquipmentItem<Repository.Objects.Packs.CarPackExteriorColourType>, ICarPackExteriorColourType
    {
        public CarPackExteriorColourType(Repository.Objects.Packs.CarPackExteriorColourType repositoryCarPackExteriorColourType, Publication publication, Guid carId, Context context, IAssetFactory assetFactory)
            : base(repositoryCarPackExteriorColourType, publication, carId, context, assetFactory)
        {

        }

        public IReadOnlyList<IColourCombinationInfo> ColourCombinations
        {
            get { throw new NotImplementedException(); }
        }
    }
}
