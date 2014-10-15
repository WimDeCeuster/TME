using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class EngineFactory : IEngineFactory
    {
        private readonly IEngineService _engineService;
        private readonly IAssetFactory _assetFactory;

        public EngineFactory(IEngineService engineService, IAssetFactory assetFactory)
        {
            _engineService = engineService;
            _assetFactory = assetFactory;
        }

        public IEnumerable<IEngine> GetEngines(Publication publication, Context context)
        {
            return _engineService.GetEngines(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(engine => new Engine(engine, publication, context, _assetFactory))
                                 .ToArray();
        }
    }
}
