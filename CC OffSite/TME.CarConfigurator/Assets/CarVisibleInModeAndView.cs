using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.Assets
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

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetCarAssets(RepositoryPublication, _carID, ObjectID, RepositoryContext, View, Mode);
        }
    }
}
