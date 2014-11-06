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
        private IReadOnlyList<IAsset> _fetchedAssets;
        private IGradeEquipment _fetchedEquipment;
        private IReadOnlyList<IGradePack> _fetchedPacks;
        private IReadOnlyList<IVisibleInModeAndView> _fetchedVisibleInModeAndViews;
        private Price _price;

        protected readonly Repository.Objects.Publication RepositoryPublication;
        protected readonly Repository.Objects.Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;
        protected readonly IPackFactory PackFactory;
        protected readonly IEquipmentFactory EquipmentFactory;

        public Grade(Repository.Objects.Grade repositoryGrade, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory, IEquipmentFactory equipmentFactory, IPackFactory packFactory)
            : base(repositoryGrade)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (packFactory == null) throw new ArgumentNullException("packFactory");

            RepositoryPublication = repositoryPublication;
            RepositoryContext = repositoryContext;
            AssetFactory = assetFactory;
            EquipmentFactory = equipmentFactory;
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
                return _fetchedVisibleInModeAndViews = _fetchedVisibleInModeAndViews ?? FetchVisibleInModeAndViews();
            }
        }

        public IReadOnlyList<IAsset> Assets { get { return _fetchedAssets = _fetchedAssets ?? FetchAssets(); } }

        public IGradeEquipment Equipment
        {
            get { return _fetchedEquipment = _fetchedEquipment ?? FetchEquipment(); }
        }


        public IReadOnlyList<IGradePack> Packs
        {
            get { return _fetchedPacks = _fetchedPacks ?? FetchPacks(); }
        }

        protected virtual IReadOnlyList<VisibleInModeAndView> FetchVisibleInModeAndViews()
        {
            return RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
        }

        protected virtual IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetAssets(RepositoryPublication, ID, RepositoryContext);
        }

        protected virtual IGradeEquipment FetchEquipment()
        {
            return EquipmentFactory.GetGradeEquipment(RepositoryPublication, RepositoryContext, ID);
        }

        protected virtual IReadOnlyList<IGradePack> FetchPacks()
        {
            return PackFactory.GetGradePacks(RepositoryPublication, RepositoryContext, RepositoryObject.ID);
        }
    }
}
