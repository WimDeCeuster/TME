using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;

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

        private IEnumerable<IAsset> _assets;
        private readonly IDictionary<string, IEnumerable<IAsset>> _modeAndViewAssets = new Dictionary<string, IEnumerable<IAsset>>();

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
        public bool VisibleInExteriorSpin { get { return _repositoryEngine.VisibleInExteriorSpin; } }
        public bool VisibleInInteriorSpin { get { return _repositoryEngine.VisibleInInteriorSpin; } }
        public bool VisibleInXRay4X4Spin { get { return _repositoryEngine.VisibleInXRay4X4Spin; } }
        public bool VisibleInXRayHybridSpin { get { return _repositoryEngine.VisibleInXRayHybridSpin; } }
        public bool VisibleInXRaySafetySpin { get { return _repositoryEngine.VisibleInXRaySafetySpin; } }
        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext); } }
        public IEnumerable<IAsset> GetAssets(string view, string mode)
        {
            var key = string.Format("{0}{1}", view, mode);
            
            if (_modeAndViewAssets.ContainsKey(key)) return _modeAndViewAssets[key];

            var assets = _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext, view, mode);

            _modeAndViewAssets.Add(key, assets);

            return assets;
        }
    }
}