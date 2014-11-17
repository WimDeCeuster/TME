using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class SubModelGrade : Grade
    {
        private readonly Guid _subModelID;

        public SubModelGrade(Repository.Objects.Grade repositoryGrade, Publication repositoryPublication, Context repositoryContext, Guid subModelID, IAssetFactory assetFactory, IEquipmentFactory equipmentFactory, IPackFactory packFactory)
            : base(repositoryGrade, repositoryPublication, repositoryContext, assetFactory, equipmentFactory, packFactory)
        {
            _subModelID = subModelID;
        }

        protected override IReadOnlyList<VisibleInModeAndView> FetchVisibleInModeAndViews()
        {
            return RepositoryObject.VisibleIn.Select(
                            visibleIn =>
                                new SubModelVisibleInModeAndView(_subModelID, RepositoryObject.ID, visibleIn,
                                    RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
        }

        protected override IGradeEquipment FetchEquipment()
        {
            return EquipmentFactory.GetSubModelGradeEquipment(RepositoryPublication, _subModelID, RepositoryContext, ID);
        }

        protected override IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetSubModelAssets(RepositoryPublication, _subModelID, ID, RepositoryContext);
        }

        protected override IReadOnlyList<IGradePack> FetchPacks()
        {
            return PackFactory.GetSubModelGradePacks(RepositoryPublication, RepositoryContext, _subModelID, RepositoryObject.ID);
        }
    }
}