using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Colours;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Packs
{
    public class CarPackUpholsteryType : CarPackEquipmentItem<Repository.Objects.Packs.CarPackUpholsteryType>, ICarPackUpholsteryType
    {
        private IReadOnlyList<IColourCombinationInfo> _colourCombinations;

        public CarPackUpholsteryType(Repository.Objects.Packs.CarPackUpholsteryType repositoryCarPackUpholsteryType, Publication publication, Guid carId, Context context, IAssetFactory assetFactory)
            : base(repositoryCarPackUpholsteryType, publication, carId, context, assetFactory)
        {

        }

        public IReadOnlyList<IColourCombinationInfo> ColourCombinations
        {
            get { return _colourCombinations = _colourCombinations ?? RepositoryObject.ColourCombinations.Select(cc => new ColourCombinationInfo(cc)).ToList(); }
        }
    }
}
