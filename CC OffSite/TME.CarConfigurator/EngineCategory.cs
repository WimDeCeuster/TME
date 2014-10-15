using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class EngineCategory : BaseObject, IEngineCategory
    {
        private readonly Repository.Objects.EngineCategory _repositoryEngineCategory;
        private IEnumerable<IAsset> _assets;

        public EngineCategory(Repository.Objects.EngineCategory repositoryEngineCategory)
            : base(repositoryEngineCategory)
        {
            if (repositoryEngineCategory == null) throw new ArgumentNullException("repositoryEngineCategory");
            
            _repositoryEngineCategory = repositoryEngineCategory;
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _repositoryEngineCategory.Assets.Select(asset => new Asset(asset)).ToArray(); } }
    }
}