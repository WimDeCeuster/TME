using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Assets;
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

        protected override IReadOnlyList<IVisibleInModeAndView> FetchVisibleInModeAndViews()
        {
            return
                RepositoryObject.VisibleIn.Select(
                    visibleIn =>
                        new CarVisibleInModeAndView(_carID, RepositoryObject.ID, visibleIn, RepositoryPublication,
                            RepositoryContext, AssetFactory)).ToList();
        }

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetCarAssets(RepositoryPublication, _carID, RepositoryObject.ID, RepositoryContext);
        }

        public IPrice Price { get { return new Price(((Repository.Objects.Colours.CarUpholstery) RepositoryObject).Price); } }
    }
}