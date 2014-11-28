using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TME.CarConfigurator.Colours;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Packs
{
    public class CarPackExteriorColourType : CarPackEquipmentItem<Repository.Objects.Packs.CarPackExteriorColourType>, ICarPackExteriorColourType
    {
        private IReadOnlyList<IColourCombinationInfo> _colourCombinations;

        public CarPackExteriorColourType(Repository.Objects.Packs.CarPackExteriorColourType repositoryCarPackExteriorColourType, Publication publication, Guid carId, Context context, IAssetFactory assetFactory, IRuleFactory ruleFactory)
            : base(repositoryCarPackExteriorColourType, publication, carId, context, assetFactory, ruleFactory)
        {

        }

        public IReadOnlyList<IColourCombinationInfo> ColourCombinations
        {
            get { return _colourCombinations = _colourCombinations ?? RepositoryObject.ColourCombinations.Select(cc => new ColourCombinationInfo(cc)).ToList(); }
        }
    }
}
