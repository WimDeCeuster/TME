using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;

namespace TME.CarConfigurator
{
    public class Grade : BaseObject<Repository.Objects.Grade>, IGrade
    {
        private readonly IPackFactory _packFactory;
        protected readonly Repository.Objects.Publication RepositoryPublication;
        protected readonly Repository.Objects.Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;
        protected readonly IEquipmentFactory GradeEquipmentFactory;
        protected IReadOnlyList<IAsset> FetchedAssets;
        protected IGradeEquipment _equipment;
        protected IReadOnlyList<IGradeEquipmentItem> EquipmentItems;
        protected IReadOnlyList<IVisibleInModeAndView> FetchedVisibleInModeAndViews;
        private IReadOnlyList<IGradePack> _packs;

        Price _price;

        public Grade(Repository.Objects.Grade repositoryGrade, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory, IEquipmentFactory gradeEquipmentFactory, IPackFactory packFactory)
            : base(repositoryGrade)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (packFactory == null) throw new ArgumentNullException("packFactory");

            RepositoryPublication = repositoryPublication;
            RepositoryContext = repositoryContext;
            AssetFactory = assetFactory;
            GradeEquipmentFactory = gradeEquipmentFactory;
            _packFactory = packFactory;
        }

        public bool Special { get { return RepositoryObject.Special; } }

   

        public IPrice StartingPrice { get { return _price = _price ?? new Price(RepositoryObject.StartingPrice); } }

        public IGradeInfo BasedUpon
        {
            get
            {
                return RepositoryObject.BasedUpon == null ? null : new GradeInfo(RepositoryObject.BasedUpon);
            }
        }

        public virtual IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return FetchedVisibleInModeAndViews = FetchedVisibleInModeAndViews ?? RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
            }
        }

        public virtual IReadOnlyList<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetAssets(RepositoryPublication, ID, RepositoryContext); } }

        public virtual IGradeEquipment Equipment
        {
            get { return _equipment = _equipment ?? GradeEquipmentFactory.GetGradeEquipment(RepositoryPublication, RepositoryContext, ID); }
        }


        public IReadOnlyList<IGradePack> Packs
        {
            get { return _packs = _packs ?? _packFactory.GetGradePacks(RepositoryPublication, RepositoryContext, RepositoryObject.ID); }
        }
    }
}
