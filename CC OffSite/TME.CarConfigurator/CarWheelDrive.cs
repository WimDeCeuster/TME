using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarWheelDrive : WheelDrive
    {
        private readonly Guid _carID;

        public CarWheelDrive(Repository.Objects.WheelDrive repositoryWheelDrive, Publication repositoryPublication, Guid carID,Context repositoryContext, IAssetFactory assetFactory) 
            : base(repositoryWheelDrive, repositoryPublication, repositoryContext, assetFactory)
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