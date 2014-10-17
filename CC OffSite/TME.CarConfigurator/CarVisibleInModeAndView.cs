using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class CarVisibleInModeAndView : VisibleInModeAndView
    {
        readonly Guid _carID;

        public CarVisibleInModeAndView(
            Guid carID,
            Guid objectID,
            Repository.Objects.Assets.VisibleInModeAndView repositoryVisibleInModeAndView,
            Repository.Objects.Publication repositoryPublication,
            Repository.Objects.Context repositoryContext, 
            IAssetFactory assetFactory)
            : base(objectID, repositoryVisibleInModeAndView, repositoryPublication, repositoryContext, assetFactory)
        {
            _carID = carID;
        }

        public override IEnumerable<IAsset> Assets
        {
            get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetCarAssets(RepositoryPublication, _carID, ObjectID, RepositoryContext, View, Mode); }
        }
    }
}
