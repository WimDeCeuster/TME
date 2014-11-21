using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Assets
{
    public class CarEquipmentVisibleInModeAndView : CarVisibleInModeAndView
    {
        public CarEquipmentVisibleInModeAndView(Guid carID, Guid objectID, Repository.Objects.Assets.VisibleInModeAndView repositoryVisibleInModeAndView, Publication repositoryPublication, Context repositoryContext, IAssetFactory assetFactory) 
            : base(carID, objectID, repositoryVisibleInModeAndView, repositoryPublication, repositoryContext, assetFactory)
        {
        }

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetCarEquipmentAssets(RepositoryPublication, CarID, ObjectID, RepositoryContext, View, Mode);
        }
    }
}