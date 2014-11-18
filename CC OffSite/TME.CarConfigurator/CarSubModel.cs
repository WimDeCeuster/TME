using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class CarSubModel : SubModel
    {
        private readonly Guid _carID;

        public CarSubModel(Repository.Objects.SubModel repositorySubModel, Publication repositoryPublication, Guid carID, Context repositoryContext, IAssetFactory assetFactory, IGradeFactory gradeFactory) 
            : base(repositorySubModel, repositoryPublication, repositoryContext, assetFactory, gradeFactory)
        {
            _carID = carID;
        }

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetCarAssets(RepositoryPublication, _carID, ID, RepositoryContext);
        }
    }
}