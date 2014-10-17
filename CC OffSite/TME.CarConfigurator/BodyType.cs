using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;


namespace TME.CarConfigurator
{
    public class BodyType : BaseObject, IBodyType
    {
        protected readonly Repository.Objects.BodyType RepositoryBodyType;
        protected readonly Repository.Objects.Publication RepositoryPublication;
        protected readonly Repository.Objects.Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;
        protected IEnumerable<IAsset> FetchedAssets;
        protected IEnumerable<IVisibleInModeAndView> FetchedVisibleInModeAndViews;

        public BodyType(Repository.Objects.BodyType repositoryBodyType, Repository.Objects.Publication publication, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory)
            : base(repositoryBodyType)
        {
            if (repositoryBodyType == null) throw new ArgumentNullException("repositoryBodyType");
            if (publication == null) throw new ArgumentNullException("publication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            RepositoryBodyType = repositoryBodyType;
            RepositoryPublication = publication;
            RepositoryContext = repositoryContext;
            AssetFactory = assetFactory;
        }

        public int NumberOfDoors { get { return RepositoryBodyType.NumberOfDoors; } }
        public int NumberOfSeats { get { return RepositoryBodyType.NumberOfSeats; } }

        public virtual IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return
                    FetchedVisibleInModeAndViews = FetchedVisibleInModeAndViews ?? RepositoryBodyType.VisibleIn.Select(x => new VisibleInModeAndView(RepositoryBodyType.ID, x, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();

            }
        }

        public virtual IEnumerable<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetAssets(RepositoryPublication, ID, RepositoryContext); } }

        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInExteriorSpin { get { return VisibleIn.VisibleInExteriorSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInInteriorSpin { get { return VisibleIn.VisibleInInteriorSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRay4X4Spin { get { return VisibleIn.VisibleInXRay4X4Spin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRayHybridSpin { get { return VisibleIn.VisibleInXRayHybridSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRaySafetySpin { get { return VisibleIn.VisibleInXRaySafetySpin(); } }
    }
}