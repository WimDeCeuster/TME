using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class Grade : BaseObject, IGrade
    {
        protected readonly Repository.Objects.Grade RepositoryGrade;
        protected readonly Repository.Objects.Publication RepositoryPublication;
        protected readonly Repository.Objects.Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;
        protected IEnumerable<IAsset> FetchedAssets;
        protected IEnumerable<IVisibleInModeAndView> FetchedVisibleInModeAndViews;

        Price _price;
        IGrade _basedUponGrade;

        public Grade(Repository.Objects.Grade repositoryGrade, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IGrade basedUponGrade, IAssetFactory assetFactory)
            : base(repositoryGrade)
        {
            if (repositoryGrade == null) throw new ArgumentNullException("repositoryGrade");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            RepositoryGrade = repositoryGrade;
            RepositoryPublication = repositoryPublication;
            RepositoryContext = repositoryContext;
            _basedUponGrade = basedUponGrade;
            AssetFactory = assetFactory;
        }

        public bool Special { get { return RepositoryGrade.Special; } }

        public IGrade BasedUpon { get { return _basedUponGrade; } }

        public IPrice StartingPrice { get { return _price = _price ?? new Price(RepositoryGrade.StartingPrice); } }

        public virtual IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return FetchedVisibleInModeAndViews = FetchedVisibleInModeAndViews ?? RepositoryGrade.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryGrade.ID, visibleInModeAndView, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
            }
        }

        public virtual IEnumerable<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetAssets(RepositoryPublication, ID, RepositoryContext); } }

        public IEnumerable<Interfaces.Equipment.IGradeEquipmentItem> Equipment
        {
            get { throw new NotImplementedException(); }
        }
    }
}
