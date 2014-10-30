using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class EngineCategory : BaseObject<Repository.Objects.EngineCategory>, IEngineCategory
    {
        private IEnumerable<IAsset> _assets;

        public EngineCategory(Repository.Objects.EngineCategory repositoryEngineCategory)
            : base(repositoryEngineCategory)
        {
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? RepositoryObject.Assets.Select(asset => new Asset(asset)).ToArray(); } }
    }
}