using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Publication _repositoryPublication;
        private readonly Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;

        private ITransmissionType _type;

        private IEnumerable<IVisibleInModeAndView> _visibleInModeAndViews;
        private IEnumerable<IAsset> _assets;

        public Transmission(Repository.Objects.Transmission transmission, Publication repositoryPublication, Context repositoryContext, IAssetFactory assetFactory)
            : base(transmission)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }

        public ITransmissionType Type { get { return _type = _type ?? new TransmissionType(RepositoryObject.Type); } }
        public Boolean KeyFeature { get { return RepositoryObject.KeyFeature; } }
        public Boolean Brochure { get { return RepositoryObject.Brochure; } }
        public Int32 NumberOfGears { get { return RepositoryObject.NumberOfGears; } }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return _visibleInModeAndViews = _visibleInModeAndViews ?? RepositoryObject.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(RepositoryObject.ID, visibleInModeAndView, _repositoryPublication, _repositoryContext, _assetFactory)).ToList();
            }
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext); } }

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