using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class Transmission : BaseObject<Repository.Objects.Transmission>, ITransmission
    {
        protected readonly Publication RepositoryPublication;
        protected readonly Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;

        private IReadOnlyList<IAsset> _fetchedAssets;
        private IReadOnlyList<IVisibleInModeAndView> _fetchedVisibleInModeAndViews;

        private ITransmissionType _type;

        public Transmission(Repository.Objects.Transmission transmission, Publication repositoryPublication, Context repositoryContext, IAssetFactory assetFactory)
            : base(transmission)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            RepositoryPublication = repositoryPublication;
            RepositoryContext = repositoryContext;
            AssetFactory = assetFactory;
        }

        public ITransmissionType Type { get { return _type = _type ?? new TransmissionType(RepositoryObject.Type); } }
        public Boolean KeyFeature { get { return RepositoryObject.KeyFeature; } }
        public Boolean Brochure { get { return RepositoryObject.Brochure; } }
        public Int32 NumberOfGears { get { return RepositoryObject.NumberOfGears; } }

        public IReadOnlyList<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return _fetchedVisibleInModeAndViews = _fetchedVisibleInModeAndViews ?? FetchVisibleInModeAndViews();
            }
        }

        public IReadOnlyList<IAsset> Assets { get { return _fetchedAssets = _fetchedAssets ?? FetchAssets(); } }

        protected  virtual IReadOnlyList<VisibleInModeAndView> FetchVisibleInModeAndViews()
        {
            return RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, RepositoryPublication, RepositoryContext, AssetFactory)).ToList();
        }

        protected virtual IReadOnlyList<IAsset> FetchAssets()
        {
            return AssetFactory.GetAssets(RepositoryPublication, ID, RepositoryContext);
        }

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