using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Assets
{
    public class CarEquipmentVisibleInModeAndView : VisibleInModeAndView
    {
        private readonly Guid _carID;

        public CarEquipmentVisibleInModeAndView(Guid carID, Guid objectID, Repository.Objects.Assets.VisibleInModeAndView repositoryVisibleInModeAndView, Publication repositoryPublication, Context repositoryContext, IAssetFactory assetFactory) 
            : base(objectID, repositoryVisibleInModeAndView, repositoryPublication, repositoryContext, assetFactory)
        {
            _carID = carID;
        }

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetCarEquipmentAssets(RepositoryPublication, _carID, ObjectID, RepositoryContext, View, Mode);
        }
    }
}