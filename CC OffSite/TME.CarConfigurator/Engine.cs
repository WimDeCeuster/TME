using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class Engine : IEngine
    {
        private readonly Repository.Objects.Engine _engine;
        private IEnumerable<Label> _labels;
        private EngineCategory _category;
        private EngineType _type;

        public Engine(Repository.Objects.Engine engine)
        {
            if (engine == null) throw new ArgumentNullException("engine");
            _engine = engine;
        }
        public Guid ID { get { return _engine.ID; } }
        public string InternalCode { get { return _engine.InternalCode; } }
        public string LocalCode { get { return _engine.LocalCode; } }
        public string Name { get { return _engine.Name; } }
        public string Description { get { return _engine.Description; } }
        public string FootNote { get { return _engine.FootNote; } }
        public string ToolTip { get { return _engine.ToolTip; } }
        public int SortIndex { get { return _engine.SortIndex; } }
        public IEnumerable<ILabel> Labels { get { return _labels = _labels ?? _engine.Labels.Select(label => new Label(label)); } }
        public IEngineType Type { get { return _type = _type ?? new EngineType(_engine.Type); } }
        public IEngineCategory Category { get { return _engine.Category == null ? null : _category = _category ?? new EngineCategory(_engine.Category); } }
        public bool KeyFeature { get { return _engine.KeyFeature; } }
        public bool Brochure { get { return _engine.Brochure; } }
        public bool VisibleInExteriorSpin { get { return _engine.VisibleInExteriorSpin; } }
        public bool VisibleInInteriorSpin { get { return _engine.VisibleInInteriorSpin; } }
        public bool VisibleInXRay4X4Spin { get { return _engine.VisibleInXRay4X4Spin; } }
        public bool VisibleInXRayHybridSpin { get { return _engine.VisibleInXRayHybridSpin; } }
        public bool VisibleInXRaySafetySpin { get { return _engine.VisibleInXRaySafetySpin; } }
        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }
    }
}