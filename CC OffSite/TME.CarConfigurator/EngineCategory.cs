using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class EngineCategory : IEngineCategory
    {
        private readonly Repository.Objects.EngineCategory _engineCategory;
        private IEnumerable<Label> _labels;

        public EngineCategory(Repository.Objects.EngineCategory engineCategory)
        {
            if (engineCategory == null) throw new ArgumentNullException("engineCategory");
            _engineCategory = engineCategory;
        }
        public Guid ID { get { return _engineCategory.ID; } }
        public string InternalCode { get { return _engineCategory.InternalCode; } }
        public string LocalCode { get { return _engineCategory.LocalCode; } }
        public string Name { get { return _engineCategory.Name; } }
        public string Description { get { return _engineCategory.Description; } }
        public string FootNote { get { return _engineCategory.FootNote; } }
        public string ToolTip { get { return _engineCategory.ToolTip; } }
        public int SortIndex { get { return _engineCategory.SortIndex; } }
        public IEnumerable<ILabel> Labels { get { return _labels = _labels ?? _engineCategory.Labels.Select(label => new Label(label) ); } }
        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }
    }
}