using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Extensions;

namespace TME.CarConfigurator
{
    public class CarEngine : Engine
    {
        readonly Guid _carId;

        public CarEngine(Repository.Objects.Engine repositoryEngine, Repository.Objects.Publication publication, Guid carId, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory)
            : base(repositoryEngine, publication, repositoryContext, assetFactory)
        {
            _carId = carId;
        }

        public override IEnumerable<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetCarAssets(RepositoryPublication, _carId, ID, RepositoryContext); } }

        public override IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return FetchedVisibleInModeAndViews = FetchedVisibleInModeAndViews ??
                                                      RepositoryEngine.VisibleIn.Select(visibleIn =>
                                                          new CarVisibleInModeAndView(_carId, RepositoryEngine.ID, visibleIn, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
            }
        }
    }
}