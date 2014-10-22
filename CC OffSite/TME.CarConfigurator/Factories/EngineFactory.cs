using System;
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
            if (engineService == null) throw new ArgumentNullException("engineService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            
            _engineService = engineService;
            _assetFactory = assetFactory;
        }

        public IReadOnlyList<IEngine> GetEngines(Publication publication, Context context)
        {
            return _engineService.GetEngines(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(engine => new Engine(engine, publication, context, _assetFactory))
                                 .ToArray();
        }

        public IEngine GetCarEngine(Repository.Objects.Engine engine, Guid carId, Publication publication, Context context)
        {
            return new CarEngine(engine, publication, carId, context, _assetFactory);
        }
    }
}
