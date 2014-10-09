using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.CommandServices.Interfaces;
using TME.CarConfigurator.S3.Shared;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class EngineService : IEngineService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public EngineService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service ?? new Service(null);
            _serialiser = serialiser ?? new Serialiser();
            _keyManager = keyManager ?? new KeyManager();
        }

        public async Task<IEnumerable<Result>> PutGenerationEngines(IContext context)
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

            var Engines = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => data.GenerationEngines.Where(engine =>
                                                                            timeFrame.Cars.Any(car => car.Engine.ID == engine.ID))
                                                                     .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in Engines)
                tasks.Add(PutTimeFrameGenerationEngines(brand, country, publication, entry.Key, entry.Value));

            return await Task.WhenAll(tasks);
        }

        async Task<Result> PutTimeFrameGenerationEngines(String brand, String country, Publication publication, PublicationTimeFrame timeFrame, IEnumerable<Engine> engines)
        {
            var path = _keyManager.GetGenerationEnginesKey(publication.ID, timeFrame.ID);
            var value = _serialiser.Serialise(engines);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}