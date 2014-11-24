using System;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Colours
{
    public class CarUpholstery : Upholstery, ICarUpholstery
    {
        private readonly Guid _carID;

        public CarUpholstery(Guid carID, Repository.Objects.Colours.CarUpholstery repositoryUpholstery, Publication publication, Context context, IAssetFactory assetFactory) 
            : base(repositoryUpholstery, publication, context, assetFactory)
        {
            _carID = carID;
        }

        public IPrice Price { get { return new Price(((Repository.Objects.Colours.CarUpholstery) RepositoryObject).Price); } }
    }
}