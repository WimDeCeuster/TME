using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class CarBodyType : BodyType
    {
        readonly Guid _carId;

        public CarBodyType(Repository.Objects.BodyType repositoryBodyType, Repository.Objects.Publication publication, Guid carId, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory)
            : base(repositoryBodyType, publication, repositoryContext, assetFactory)
        {
            _carId = carId;
        }

        public override IReadOnlyList<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetCarAssets(RepositoryPublication, _carId, ID, RepositoryContext); } }

        public override IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return FetchedVisibleInModeAndViews = FetchedVisibleInModeAndViews ??
                                                      RepositoryObject.VisibleIn.Select(visibleIn =>
                                                          new CarVisibleInModeAndView(_carId, RepositoryObject.ID, visibleIn, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
            }
        }
    }
}
