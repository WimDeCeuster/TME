using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class SubModelGrade : Grade
    {
        private readonly Guid _subModelID;

        public SubModelGrade(Repository.Objects.Grade repositoryGrade, Publication repositoryPublication, Context repositoryContext,Guid subModelID, IGrade basedUponGrade, IAssetFactory assetFactory, IEquipmentFactory gradeEquipmentFactory, IPackFactory packFactory) 
            : base(repositoryGrade, repositoryPublication, repositoryContext, basedUponGrade, assetFactory, gradeEquipmentFactory, packFactory)
        {
            _subModelID = subModelID;
        }

        public override IReadOnlyList<IAsset> Assets
        {
            get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetSubModelAssets(RepositoryPublication, _subModelID, ID, RepositoryContext); } }

        public override IGradeEquipment Equipment
        {
            get { return _equipment = _equipment ?? GradeEquipmentFactory.GetSubModelGradeEquipment(RepositoryPublication,_subModelID, RepositoryContext, ID); }
        }
    }
}