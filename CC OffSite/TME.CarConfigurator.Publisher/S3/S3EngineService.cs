using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3EngineService : IS3EngineService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public S3EngineService(IService service, ISerialiser serialiser, IKeyManager keyManager)
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

        async Task<IEnumerable<Result>> PutTimeFramesGenerationEngines(String brand, String country, IReadOnlyList<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var Engines = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => data.GenerationEngines.Where(Engine =>
                                                                            timeFrame.Cars.Any(car => car.Engine.ID == Engine.ID))
                                                                     .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in Engines)
                tasks.Add(PutTimeFrameGenerationEngines(brand, country, publication, entry.Key, entry.Value));

            return await Task.WhenAll(tasks);
        }

        async Task<Result> PutTimeFrameGenerationEngines(String brand, String country, Publication publication, PublicationTimeFrame timeFrame, IEnumerable<Engine> engines)
        {
            var path = _keyManager.GetGenerationEnginesKey(publication, timeFrame);
            var value = _serialiser.Serialise(engines);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}