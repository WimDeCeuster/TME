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

        public override IEnumerable<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetCarAssets(RepositoryPublication, _carID, RepositoryObject.ID, RepositoryContext); } }

        public override IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return FetchedVisibleInModeAndViews = FetchedVisibleInModeAndViews ??
                                                      RepositoryObject.VisibleIn.Select(visibleIn =>
                                                          new CarVisibleInModeAndView(_carID, RepositoryObject.ID,
                                                              visibleIn, RepositoryPublication, RepositoryContext,
                                                              AssetFactory)).ToList();
            }
        }
    }
}