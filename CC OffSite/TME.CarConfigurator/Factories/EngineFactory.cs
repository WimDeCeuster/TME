using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.Factories
{
    public class EngineFactory : IEngineFactory
    {
        private IEngineService _engineService;

        public EngineFactory(QueryServices.IEngineService engineService)
        {
            _engineService = engineService;
        }
        public IEnumerable<IEngine> GetEngines(Repository.Objects.Publication publication, Repository.Objects.Context context)
        {
            return _engineService.GetEngines(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(engine => new Engine(engine));
        }
    }
}
