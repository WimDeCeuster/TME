using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Extensions;

namespace TME.CarConfigurator
{
    public class Engine : BaseObject, IEngine
    {
        private readonly Repository.Objects.Engine _repositoryEngine;
        private readonly Repository.Objects.Publication _repositoryPublication;
        private readonly Repository.Objects.Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;

        private IEngineCategory _category;
        private IEngineType _type;

        private IEnumerable<IVisibleInModeAndView> _visibleInModeAndViews;
        private IEnumerable<IAsset> _assets;

        public Engine(Repository.Objects.Engine repositoryEngine, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IAssetFactory assetFactory)
            : base(repositoryEngine)
        {
            if (repositoryEngine == null) throw new ArgumentNullException("repositoryEngine");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _repositoryEngine = repositoryEngine;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
        }

        public IEngineType Type { get { return _type = _type ?? new EngineType(_repositoryEngine.Type); } }
        public IEngineCategory Category { get { return _repositoryEngine.Category == null ? null : _category = _category ?? new EngineCategory(_repositoryEngine.Category, _repositoryContext); } }
        public bool KeyFeature { get { return _repositoryEngine.KeyFeature; } }
        public bool Brochure { get { return _repositoryEngine.Brochure; } }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                return _visibleInModeAndViews = _visibleInModeAndViews ?? _repositoryEngine.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(_repositoryEngine.ID, visibleInModeAndView, _repositoryPublication, _repositoryContext, _assetFactory)).ToList();
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