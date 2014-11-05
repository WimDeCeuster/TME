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
        private readonly IAssetFactory _assetFactory;
        protected readonly Repository.Objects.Publication RepositoryPublication;
        protected readonly Repository.Objects.Context RepositoryContext;
        protected readonly IPackFactory PackFactory;
        protected readonly IEquipmentFactory GradeEquipmentFactory;

        private IReadOnlyList<IAsset> _fetchedAssets;
        private IReadOnlyList<IVisibleInModeAndView> _fetchedVisibleInModeAndViews;
        protected IGradeEquipment FetchedEquipment;
        protected IReadOnlyList<IGradePack> FetchedPacks;

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
            _assetFactory = assetFactory;
            GradeEquipmentFactory = gradeEquipmentFactory;
            PackFactory = packFactory;
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
                return _fetchedVisibleInModeAndViews = _fetchedVisibleInModeAndViews ?? RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, RepositoryPublication, RepositoryContext, _assetFactory)).ToList();
            }
        }

        public virtual IReadOnlyList<IAsset> Assets { get { return _fetchedAssets = _fetchedAssets ?? _assetFactory.GetAssets(RepositoryPublication, ID, RepositoryContext); } }

        public virtual IGradeEquipment Equipment
        {
            get { return FetchedEquipment = FetchedEquipment ?? GradeEquipmentFactory.GetGradeEquipment(RepositoryPublication, RepositoryContext, ID); }
        }


        public virtual IReadOnlyList<IGradePack> Packs
        {
            get { return FetchedPacks = FetchedPacks ?? PackFactory.GetGradePacks(RepositoryPublication, RepositoryContext, RepositoryObject.ID); }
        }
    }
}
