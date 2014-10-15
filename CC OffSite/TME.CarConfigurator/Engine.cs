using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class Engine : BaseObject, IEngine
    {
        private readonly Repository.Objects.Engine _engine;
        private readonly Repository.Objects.Context _context;
        private readonly IAssetFactory _assetFactory;
        private readonly Publication _publication;
        private IEngineCategory _category;
        private IEngineType _type;

        private IEnumerable<IAsset> _assets;
        private IDictionary<string, IEnumerable<IAsset>> _3DAssets = new Dictionary<string, IEnumerable<IAsset>>();

        public Engine(Repository.Objects.Engine engine, Publication publication, Context context, IAssetFactory assetFactory)
            : base(engine)
        {
            if (engine == null) throw new ArgumentNullException("engine");
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            _engine = engine;
            _assetFactory = assetFactory;
            _publication = publication;
            _context = context;
        }

        public IEngineType Type { get { return _type = _type ?? new EngineType(_engine.Type); } }
        public IEngineCategory Category { get { return _engine.Category == null ? null : _category = _category ?? new EngineCategory(_engine.Category); } }
        public bool KeyFeature { get { return _engine.KeyFeature; } }
        public bool Brochure { get { return _engine.Brochure; } }
        public bool VisibleInExteriorSpin { get { return _engine.VisibleInExteriorSpin; } }
        public bool VisibleInInteriorSpin { get { return _engine.VisibleInInteriorSpin; } }
        public bool VisibleInXRay4X4Spin { get { return _engine.VisibleInXRay4X4Spin; } }
        public bool VisibleInXRayHybridSpin { get { return _engine.VisibleInXRayHybridSpin; } }
        public bool VisibleInXRaySafetySpin { get { return _engine.VisibleInXRaySafetySpin; } }
        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_publication, ID, _context); } }
        public IEnumerable<IAsset> GetAssets(string view, string mode)
        {
            var key = string.Format("{0}{1}", view, mode);

            if (_3DAssets.ContainsKey(key)) return _3DAssets[key];

            var assets = _assetFactory.GetAssets(_publication, ID, _context, view, mode);

            _3DAssets.Add(key, assets);

            return assets;
        }
    }
}