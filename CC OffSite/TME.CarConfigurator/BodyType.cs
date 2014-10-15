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
        private readonly Repository.Objects.BodyType _repositoryBodyType;
        private readonly Repository.Objects.Publication _repositoryPublication;
        private readonly Repository.Objects.Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;
        private IEnumerable<IAsset> _assets;
        private readonly Dictionary<string, IEnumerable<IAsset>> _modeAndViewAssets = new Dictionary<string, IEnumerable<IAsset>>();
        private IEnumerable<IVisibleInModeAndView> _visibleInModeAndViews;

        public BodyType(Repository.Objects.BodyType repositoryBodyType, Repository.Objects.Publication publication, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory)
            : base(repositoryBodyType)
        {
            if (repositoryBodyType == null) throw new ArgumentNullException("repositoryBodyType");
            if (publication == null) throw new ArgumentNullException("publication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositoryBodyType = repositoryBodyType;
            _repositoryPublication = publication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }

        public int NumberOfDoors { get { return _repositoryBodyType.NumberOfDoors; } }
        public int NumberOfSeats { get { return _repositoryBodyType.NumberOfSeats; } }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return
                    _visibleInModeAndViews = _visibleInModeAndViews ?? _repositoryBodyType.VisibleIn.Select(x => new VisibleInModeAndView(_repositoryBodyType.ID, x, _repositoryPublication, _repositoryContext, _assetFactory));

            }
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext); } }
        public IEnumerable<IAsset> GetAssets(string view, string mode)
        {
            var key = string.Format("{0}-{1}", view, mode);

            if (_modeAndViewAssets.ContainsKey(key))
                return _modeAndViewAssets[key];

            var assets = _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext, view, mode).ToList();

            _modeAndViewAssets.Add(key, assets);

            return assets;
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