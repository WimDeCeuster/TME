using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class WheelDrive : BaseObject, IWheelDrive
    {
        readonly Repository.Objects.WheelDrive _repositoryWheelDrive;
        readonly Repository.Objects.Publication _repositoryPublication;
        readonly Repository.Objects.Context _repositoryContext;
        readonly IAssetFactory _assetFactory;

        private IEnumerable<IVisibleInModeAndView> _visibleInModeAndViews;
        private IEnumerable<IAsset> _assets;

        public WheelDrive(Repository.Objects.WheelDrive repositoryWheelDrive, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory)
            : base(repositoryWheelDrive)
        {
            if (repositoryWheelDrive == null) throw new ArgumentNullException("repositoryWheelDrive");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositoryWheelDrive = repositoryWheelDrive;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }

        public Boolean KeyFeature { get { return _repositoryWheelDrive.KeyFeature; } }

        public Boolean Brochure { get { return _repositoryWheelDrive.Brochure; } }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return _visibleInModeAndViews = _visibleInModeAndViews ?? _repositoryWheelDrive.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(_repositoryWheelDrive.ID, visibleInModeAndView, _repositoryPublication, _repositoryContext, _assetFactory)).ToList();
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
