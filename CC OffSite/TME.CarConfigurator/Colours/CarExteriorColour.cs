using System;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class CarExteriorColour : ExteriorColour, ICarExteriorColour
    {
        private readonly Guid _carID;

        public CarExteriorColour(Guid carID, Repository.Objects.Colours.CarExteriorColour repositoryColour, Publication publication, Context context, IAssetFactory assetFactory) 
            : base(repositoryColour, publication, context, assetFactory)
        {
            _carID = carID;
        }

        public IPrice Price { get { return new Price(((Repository.Objects.Colours.CarExteriorColour) RepositoryObject).Price); } }
    }
}