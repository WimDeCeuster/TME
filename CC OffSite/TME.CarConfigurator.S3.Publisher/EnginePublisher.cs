using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.Publisher
{
    public class EnginePublisher : IEnginePublisher
    {
        IEngineService _engineService;

        public EnginePublisher(IEngineService engineService)
        {
            _engineService = engineService;
        }

        public async Task<IEnumerable<Result>> PublishGenerationEngines(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutTimeFramesGenerationEngines(context.Brand, context.Country, timeFrames, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutTimeFramesGenerationEngines(String brand, String country, IEnumerable<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var engines = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => data.Engines.Where(engine => timeFrame.Cars.Any(car => car.Engine.ID == engine.ID))
                                                         .OrderBy(engine => engine.SortIndex)
                                                         .ThenBy(engine => engine.Name)
                                                         .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in engines)
                tasks.Add(_engineService.PutTimeFrameGenerationEngines(brand, country, publication.ID, entry.Key.ID, entry.Value));

            return await Task.WhenAll(tasks);
        }
    }
}
