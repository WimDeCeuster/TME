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
    public class EngineCategory : BaseObject, IEngineCategory
    {
        private readonly Repository.Objects.EngineCategory _engineCategory;
        private IEnumerable<IAsset> _assets;

        public EngineCategory(Repository.Objects.EngineCategory engineCategory)
            : base(engineCategory)
        {
            if (engineCategory == null) throw new ArgumentNullException("engineCategory");
            
            _engineCategory = engineCategory;
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _engineCategory.Assets.Select(asset => new Asset(asset)).ToArray(); } }
    }
}