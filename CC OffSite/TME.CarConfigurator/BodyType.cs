using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class BodyType : IBodyType
    {
        private readonly Repository.Objects.BodyType _bodyType;
        private readonly Publication _publication;
        private readonly Context _context;
        private readonly IAssetFactory _assetFactory;
        private IEnumerable<IAsset> _assets;
        private Dictionary<string, IEnumerable<IAsset>> _3DAssets = new Dictionary<string, IEnumerable<IAsset>>();

        public BodyType(Repository.Objects.BodyType bodyType, Publication publication, Context context, IAssetFactory assetFactory)
        {
            if (bodyType == null) throw new ArgumentNullException("bodyType");
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _bodyType = bodyType;
            _publication = publication;
            _context = context;
            _assetFactory = assetFactory;
        }

        public Guid ID { get { return _bodyType.ID; } }
        public string InternalCode { get { return _bodyType.InternalCode; } }
        public string LocalCode { get { return _bodyType.LocalCode; } }
        public string Name { get { return _bodyType.Name; } }
        public string Description { get { return _bodyType.Description; } }
        public string FootNote { get { return _bodyType.FootNote; } }
        public string ToolTip { get { return _bodyType.ToolTip; } }
        public int SortIndex { get { return _bodyType.SortIndex; } }
        public IEnumerable<ILabel> Labels { get { throw new NotImplementedException(); } }
        public int NumberOfDoors { get { return _bodyType.NumberOfDoors; } }
        public int NumberOfSeats { get { return _bodyType.NumberOfSeats; } }
        public bool VisibleInExteriorSpin { get { return _bodyType.VisibleInExteriorSpin; } }
        public bool VisibleInInteriorSpin { get { return _bodyType.VisibleInInteriorSpin; } }
        public bool VisibleInXRay4X4Spin { get { return _bodyType.VisibleInXRay4X4Spin; } }
        public bool VisibleInXRayHybridSpin { get { return _bodyType.VisibleInXRayHybridSpin; } }
        public bool VisibleInXRaySafetySpin { get { return _bodyType.VisibleInXRaySafetySpin; } }
        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_publication, ID, _context); } }
        public IEnumerable<IAsset> Get3DAssets(string view, string mode)
        {
            var key = string.Format("{0}-{1}", view, mode);

            if (_3DAssets.ContainsKey(key))
                return _3DAssets[key];

            var assets = _assetFactory.GetAssets(_publication, ID, _context, view, mode).ToList();

            _3DAssets.Add(key, assets);

            return assets;
        }
    }
}