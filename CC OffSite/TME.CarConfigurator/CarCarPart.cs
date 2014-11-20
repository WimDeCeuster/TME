using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarCarPart : CarPart
    {
        private readonly Guid _carID;

        public CarCarPart(Repository.Objects.CarPart repositoryCarPart, Publication repositoryPublication, Guid carID, Context repositoryContext, IAssetFactory assetFactory) 
            : base(repositoryCarPart, repositoryPublication, repositoryContext, assetFactory)
        {
            _carID = carID;
        }

        public override IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return
                    FetchedVisibleIn =
                        FetchedVisibleIn ??
                        RepositoryCarPart.VisibleIn.Select(
                            visibleIn =>
                                new CarPartVisibleInModeAndView(_carID, ID, visibleIn, RepositoryPublication,
                                    RepositoryContext, AssetFactory)).ToList();
            }
        }
    }
}