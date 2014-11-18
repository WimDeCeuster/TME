using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarTransmission : Transmission
    {
        private readonly Guid _carID;

        public CarTransmission(Repository.Objects.Transmission transmission, Publication repositoryPublication, Guid carID, Context repositoryContext, IAssetFactory assetFactory) 
            : base(transmission, repositoryPublication, repositoryContext, assetFactory)
        {
            _carID = carID;
        }

        protected override IReadOnlyList<VisibleInModeAndView> FetchVisibleInModeAndViews()
        {
            return RepositoryObject.VisibleIn.Select(visibleIn => new CarVisibleInModeAndView(_carID, RepositoryObject.ID, visibleIn, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
        }

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetCarAssets(RepositoryPublication, _carID, ID, RepositoryContext);
        }
    }
}