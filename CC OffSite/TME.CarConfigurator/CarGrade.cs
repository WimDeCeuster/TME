using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarGrade : Grade
    {
        private readonly Guid _carID;

        public CarGrade(Repository.Objects.Grade repositoryGrade, Publication repositoryPublication, Guid carID, Context repositoryContext, IAssetFactory assetFactory, IEquipmentFactory equipmentFactory, IPackFactory packFactory) 
            : base(repositoryGrade, repositoryPublication, repositoryContext, assetFactory, equipmentFactory, packFactory)
        {
            _carID = carID;
        }

        public override IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return
                    FetchedVisibleInModeAndViews = FetchedVisibleInModeAndViews ??
                                                        RepositoryObject.VisibleIn.Select(visibleIn =>
                                                            new CarVisibleInModeAndView(_carID, RepositoryObject.ID, visibleIn,RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
            }
        }

        public override IReadOnlyList<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetCarAssets(RepositoryPublication, _carID, RepositoryObject.ID, RepositoryContext); } }
    }
}